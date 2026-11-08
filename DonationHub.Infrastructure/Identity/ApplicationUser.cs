using DonationHub.Infrastructure.Enums;
using Microsoft.AspNetCore.Identity;

namespace DonationHub.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; }
    public Language PreferredLanguage { get; set; } = Language.EN;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}