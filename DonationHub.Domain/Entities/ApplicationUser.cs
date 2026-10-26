using DonationHub.Domain.Value_Objects;

namespace DonationHub.Domain.Entities;

public class ApplicationUser : CommonData
{
    public string Email  { get; private set; }
    public string UserName { get; private set; }
    public string FullName  { get; private set; }
    public Language PreferredLanguage { get; private set; }
    public bool IsActive { get; private set; }
    public ApplicationUser  Admin  { get; private set; } 
    public ApplicationUser CampaignManager
    public Donor.
    public ApplicationUser()
    {
    }
}