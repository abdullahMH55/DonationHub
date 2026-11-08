using System.ComponentModel.DataAnnotations;

namespace DonationHub.Application.Common.Options;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    [MinLength(32)]
    public string SecretKey { get; set; } = string.Empty;

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;
    
    [Range(10, 30)]
    public int ExpiryMinutes { get; set; } = 15;
    
    [Range(1, 10)]
    public int RefreshTokenExpiryDays { get; set; } = 7;
}