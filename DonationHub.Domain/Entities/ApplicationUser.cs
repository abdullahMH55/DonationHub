using DonationHub.Domain.Enums;

namespace DonationHub.Domain.Entities;

public class ApplicationUser : CommonData
{
    public string Email { get; private set; } = string.Empty;
    public string UserName { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public Language PreferredLanguage { get; private set; } = Language.EN;
    public bool IsActive { get; private set; } = true;
}
