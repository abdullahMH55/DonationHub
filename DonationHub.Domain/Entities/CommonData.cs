namespace DonationHub.Domain.Entities;

public abstract class CommonData
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
}
