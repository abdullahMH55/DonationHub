namespace DonationHub.Domain.Entities;

public class IdempotencyRecord
{
    //public  Key { get; private set; }
    public Guid UserId { get; private set; }
    //public RequestHash { get; private set; }
    public int ResponseStatusCode { get; private set; }
    //public  ResponseBody { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
}
