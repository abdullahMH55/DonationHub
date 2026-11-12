using DonationHub.Application.Features.Auth.DTO;

namespace DonationHub.Application.Services;

public interface IAuthService
{
    Task<AuthResponce> RegisterAsync(RegisterRequest request);
    Task<AuthResponce> LoginAsync(LoginRequest request);
    Task<AuthResponce> RefreshTokenAsync(string refreshToken);
    Task<bool> RevokeTokenAsync(string refreshToken);
}
