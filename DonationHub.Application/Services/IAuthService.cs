using DonationHub.Application.Features.Auth.DTO;
using DonationHub.Domain.ResultHandling;

namespace DonationHub.Application.Services;

public interface IAuthService
{
    Task<Result<AuthResponce>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<Result<AuthResponce>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<Result<AuthResponce>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<Result<bool>> RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<Result<CurrentUserResponse>> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<Guid>> CreateStaffAsync(CreateStaffRequest request, CancellationToken cancellationToken = default);
}
