using System.ComponentModel.DataAnnotations;

namespace DonationHub.Application.Options;

public class DonationOptions
{
    public const string SectionName = "Donation";

    public decimal MinAmount { get; set; } = 1.00m;

    public decimal MaxAmount { get; set; } = 10000.00m;

    public int PendingTimeoutMinutes { get; set; } = 30;

    [Required]
    public string DefaultCurrency { get; set; } = "USD";
}