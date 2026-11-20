using System.ComponentModel.DataAnnotations;

namespace DonationHub.Application.Features.Auth.DTO;

public class RegisterRequest 
{
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    public string PreferredLanguage { get; set; } = "en";
}

public class LoginRequest
{

    public string Email { get; set; } = string.Empty;
    
    public string Password { get; set; } = string.Empty;
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class CreateStaffRequest
{
    public string UserName { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}

public class AuthResponce
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiration { get; set; }
}

public class CurrentUserResponse
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string PreferredLanguage { get; set; } = "en";
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
}