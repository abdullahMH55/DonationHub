using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DonationHub.Application.Common.Options;
using DonationHub.Application.Features.Auth.DTO;
using DonationHub.Application.Services;
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

    public AuthService(IOptions<JwtOptions> jwtOptions, UserManager<ApplicationUser> userManager, AppDbContext context)
    {
        _jwtOptions = jwtOptions.Value;
        _userManager = userManager;
        _context = context;
    }

    public async Task<AuthResponce> RegisterAsync(RegisterRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) != null)
        {
            return new AuthResponce { Success = false, Message = "User with the same email already exists" };
        }

        var newUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = request.UserName,
            FullName = request.FullName,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(newUser, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new AuthResponce { Success = false, Message = errors };
        }

        await _userManager.AddToRoleAsync(newUser, "Donor");

        return new AuthResponce { Success = true, Message = "Successfully registered user" };
    }

    public async Task<AuthResponce> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return new AuthResponce { Success = false, Message = "Invalid email or password" };
        }

        if (!user.IsActive)
        {
            return new AuthResponce { Success = false, Message = "Account is deactivated" };
        }

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.UserName!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes),
            signingCredentials: creds
        );

        var refreshToken = GenerateRefreshToken();
        var hashedRefreshToken = HashToken(refreshToken);
        var expiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpiryDays);

        var refreshTokenEntity = RefreshToken.Create(user.Id, hashedRefreshToken, DateTime.UtcNow, expiresAt);
        await _context.RefreshTokens.AddAsync(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return new AuthResponce
        {
            Success = true,
            Message = "Successfully logged in",
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = refreshToken,
            RefreshTokenExpiration = expiresAt
        };
    }

    public async Task<AuthResponce> RefreshTokenAsync(string refreshToken)
    {
        var Token = HashToken(refreshToken);
        var existToken = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == Token);

        if (existToken == null || existToken.IsExpired(DateTime.UtcNow))
        {
            return new AuthResponce { Success = false, Message = "Invalid or expired refresh token" };
        }

        var user = await _userManager.FindByIdAsync(existToken.UserId.ToString());
        if (user == null || !user.IsActive)
        {
            return new AuthResponce { Success = false, Message = "Account is deactivated" };
        }

        var newRefreshToken = GenerateRefreshToken();
        var newHashRefreshToken = HashToken(newRefreshToken);
        var newExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpiryDays);

        var newRefreshTokenEntity = RefreshToken.Create(user.Id, newHashRefreshToken, DateTime.UtcNow, newExpiresAt);
        await _context.RefreshTokens.AddAsync(newRefreshTokenEntity);

        existToken.Revoke(DateTime.UtcNow, newRefreshTokenEntity.Id);
        await _context.SaveChangesAsync();

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes),
            signingCredentials: creds
        );

        return new AuthResponce
        {
            Success = true,
            Message = "Token refreshed successfully",
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = newRefreshToken,
            RefreshTokenExpiration = newExpiresAt
        };
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        var hashedToken = HashToken(refreshToken);
        var tokenRecord = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hashedToken);

        if (tokenRecord == null || tokenRecord.IsRevoked())
        {
            return false;
        }

        tokenRecord.Revoke(DateTime.UtcNow);
        await _context.SaveChangesAsync();
        return true;
    }

    private string GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private string HashToken(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);
        var hashBytes = SHA256.HashData(bytes);
        return Convert.ToBase64String(hashBytes);
    }
}
