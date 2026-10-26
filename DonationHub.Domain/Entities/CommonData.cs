namespace DonationHub.Domain.Entities;

public abstract class CommonData
{
    public Guid ID { get; set; }
    public DateTime CreatedAt { get; set; }
}