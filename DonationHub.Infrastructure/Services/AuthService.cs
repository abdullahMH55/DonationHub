using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DonationHub.Application.Common.Options;
using DonationHub.Application.ErrorCatalogue;
using DonationHub.Application.Features.Auth.DTO;
using DonationHub.Application.Services;
using DonationHub.Domain.Entities;
using DonationHub.Domain.Enums;
using DonationHub.Domain.Result;
using DonationHub.Domain.ResultHandling;
using DonationHub.Infrastructure.Data;
using DonationHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DonationHub.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly JwtOptions _jwtOptions;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _context;
    private readonly TimeProvider _timeProvider;

    public AuthService(
        IOptions<JwtOptions> jwtOptions,
        UserManager<ApplicationUser> userManager,
        AppDbContext context,
        TimeProvider timeProvider)
    {
        _jwtOptions = jwtOptions.Value;
        _userManager = userManager;
        _context = context;
        _timeProvider = timeProvider;
    }

    public async Task<Result<AuthResponce>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByEmailAsync(request.Email) != null)
            return Result<AuthResponce>.Failure([AuthErrors.EmailAlreadyExists]);

        if (await _userManager.FindByNameAsync(request.UserName) != null)
            return Result<AuthResponce>.Failure([AuthErrors.UserNameAlreadyExists]);

        var nowUtc = _timeProvider.GetUtcNow().UtcDateTime;

        var newUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = request.UserName,
            FullName = request.FullName,
            Email = request.Email,
            PreferredLanguage = MapLanguage(request.PreferredLanguage),
            CreatedAt = nowUtc,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(newUser, request.Password);
        if (!result.Succeeded)
        {
            return Result<AuthResponce>.Failure(
                [CommonErrors.ValidationFailed with { Errors = ToErrorDictionary(result.Errors) }]);
        }

        await _userManager.AddToRoleAsync(newUser, "Donor");

        return Result<AuthResponce>.Success(new AuthResponce
        {
            Success = true,
            Message = "Successfully registered user"
        });
    }

    public async Task<Result<AuthResponce>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            return Result<AuthResponce>.Failure([AuthErrors.InvalidCredentials]);

        if (!user.IsActive)
            return Result<AuthResponce>.Failure([AuthErrors.AccountDeactivated]);

        var roles = await _userManager.GetRolesAsync(user);
        var nowUtc = _timeProvider.GetUtcNow().UtcDateTime;

        var token = CreateJwtToken(user, roles, nowUtc);
        var refreshToken = GenerateRefreshToken();
        var hashedRefreshToken = HashToken(refreshToken);
        var expiresAt = nowUtc.AddDays(_jwtOptions.RefreshTokenExpiryDays);

        var refreshTokenEntityResult = RefreshToken.Create(user.Id, hashedRefreshToken, nowUtc, expiresAt);
        if (!refreshTokenEntityResult.IsSuccess)
            return Result<AuthResponce>.Failure([refreshTokenEntityResult.Error!]);

        var refreshTokenEntity = refreshTokenEntityResult.Value!;
        await _context.RefreshTokens.AddAsync(refreshTokenEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<AuthResponce>.Success(new AuthResponce
        {
            Success = true,
            Message = "Successfully logged in",
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = refreshToken,
            RefreshTokenExpiration = expiresAt
        });
    }

    public async Task<Result<AuthResponce>> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var token = HashToken(refreshToken);
        var existToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == token, cancellationToken);
        var nowUtc = _timeProvider.GetUtcNow().UtcDateTime;

        if (existToken == null || existToken.IsExpired(nowUtc))
            return Result<AuthResponce>.Failure([AuthErrors.RefreshTokenInvalid]);

        if (existToken.IsRevoked())
        {
            await RevokeActiveTokensAsync(existToken.UserId, nowUtc, cancellationToken);
            return Result<AuthResponce>.Failure([AuthErrors.RefreshTokenInvalid]);
        }

        var user = await _userManager.FindByIdAsync(existToken.UserId.ToString());
        if (user == null || !user.IsActive)
            return Result<AuthResponce>.Failure([AuthErrors.AccountDeactivated]);

        var newRefreshToken = GenerateRefreshToken();
        var newHashRefreshToken = HashToken(newRefreshToken);
        var newExpiresAt = nowUtc.AddDays(_jwtOptions.RefreshTokenExpiryDays);

        var newRefreshTokenResult = RefreshToken.Create(user.Id, newHashRefreshToken, nowUtc, newExpiresAt);
        if (!newRefreshTokenResult.IsSuccess)
            return Result<AuthResponce>.Failure([newRefreshTokenResult.Error!]);

        var newRefreshTokenEntity = newRefreshTokenResult.Value!;
        await _context.RefreshTokens.AddAsync(newRefreshTokenEntity, cancellationToken);

        existToken.Revoke(nowUtc, newRefreshTokenEntity.Id);
        await _context.SaveChangesAsync(cancellationToken);

        var roles = await _userManager.GetRolesAsync(user);
        var jwtToken = CreateJwtToken(user, roles, nowUtc);

        return Result<AuthResponce>.Success(new AuthResponce
        {
            Success = true,
            Message = "Token refreshed successfully",
            Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            RefreshToken = newRefreshToken,
            RefreshTokenExpiration = newExpiresAt
        });
    }

    public async Task<Result<bool>> RevokeTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var hashedToken = HashToken(refreshToken);
        var tokenRecord = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == hashedToken, cancellationToken);

        if (tokenRecord == null || tokenRecord.IsRevoked())
            return Result<bool>.Failure([AuthErrors.RefreshTokenInvalid]);

        var nowUtc = _timeProvider.GetUtcNow().UtcDateTime;
        tokenRecord.Revoke(nowUtc);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<CurrentUserResponse>> GetCurrentUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return Result<CurrentUserResponse>.Failure([AuthErrors.InvalidCredentials]);

        var roles = await _userManager.GetRolesAsync(user);

        return Result<CurrentUserResponse>.Success(new CurrentUserResponse
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            UserName = user.UserName,
            PreferredLanguage = user.PreferredLanguage.ToString().ToLowerInvariant(),
            Roles = roles.ToList()
        });
    }

    public async Task<Result<Guid>> CreateStaffAsync(
        CreateStaffRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.Role != "Admin" && request.Role != "CampaignManager")
            return Result<Guid>.Failure([AuthErrors.InvalidRole]);

        if (await _userManager.FindByEmailAsync(request.Email) != null)
            return Result<Guid>.Failure([AuthErrors.EmailAlreadyExists]);

        if (await _userManager.FindByNameAsync(request.UserName) != null)
            return Result<Guid>.Failure([AuthErrors.UserNameAlreadyExists]);

        var nowUtc = _timeProvider.GetUtcNow().UtcDateTime;
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = request.UserName,
            Email = request.Email,
            FullName = request.FullName,
            PreferredLanguage = Language.EN,
            IsActive = true,
            CreatedAt = nowUtc
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return Result<Guid>.Failure(
                [AuthErrors.StaffCreationFailed with { Errors = ToErrorDictionary(result.Errors) }]);
        }

        await _userManager.AddToRoleAsync(user, request.Role);

        return Result<Guid>.Success(user.Id);
    }

    private async Task RevokeActiveTokensAsync(
        Guid userId,
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        var activeTokens = await _context.RefreshTokens
            .Where(token => token.UserId == userId && token.RevokedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var activeToken in activeTokens)
        {
            activeToken.Revoke(nowUtc);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    private static Language MapLanguage(string? preferredLanguage)
    {
        return preferredLanguage?.Trim().ToLowerInvariant() switch
        {
            "ar" => Language.AR,
            _ => Language.EN
        };
    }

    private static IReadOnlyDictionary<string, string[]> ToErrorDictionary(IEnumerable<IdentityError> errors)
    {
        return errors
            .GroupBy(error => error.Code)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.Description).ToArray());
    }

    private JwtSecurityToken CreateJwtToken(
        ApplicationUser user,
        IList<string> roles,
        DateTime nowUtc)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        return new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: nowUtc.AddMinutes(_jwtOptions.ExpiryMinutes),
            signingCredentials: credentials);
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private static string HashToken(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);
        var hashBytes = SHA256.HashData(bytes);
        return Convert.ToBase64String(hashBytes);
    }
}
