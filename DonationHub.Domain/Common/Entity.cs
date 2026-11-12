namespace DonationHub.Domain.Common;

public abstract class Entity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public override bool Equals(object? obj)
    {
        return obj is Entity entity && Id == entity.Id;
    }
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
    public static bool operator ==(Entity? lhs, Entity? rhs)
    {
        if (ReferenceEquals(lhs, rhs))
            return true;
        if (lhs is null || rhs is null)
            return false;
        return lhs.Id == rhs.Id;
    }

    public static bool operator !=(Entity? lhs, Entity? rhs)
    {
        return !(lhs == rhs);
    }
}
