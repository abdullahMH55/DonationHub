using DonationHub.Domain.Common;
using DonationHub.Domain.Entities;
using FluentAssertions;

namespace DonationHub.UnitTests.Domain;

public class EntityTests
{
    [Fact]
    public void EqualityOperator_HandlesNulls_WithoutRecursion()
    {
        Entity left = new Category { Id = Guid.NewGuid() };
        Entity? right = null;

        (left == right).Should().BeFalse();
        (left != right).Should().BeTrue();
        (right == null).Should().BeTrue();
        (right != null).Should().BeFalse();
    }

    [Fact]
    public void EqualityOperator_ReturnsTrue_ForEntitiesWithSameId()
    {
        var id = Guid.NewGuid();
        Entity left = new Category { Id = id };
        Entity right = new Category { Id = id };

        (left == right).Should().BeTrue();
        left.Equals(right).Should().BeTrue();
        left.GetHashCode().Should().Be(right.GetHashCode());
    }

    [Fact]
    public void EqualityOperator_ReturnsFalse_ForEntitiesWithDifferentIds()
    {
        Entity left = new Category { Id = Guid.NewGuid() };
        Entity right = new Category { Id = Guid.NewGuid() };

        (left == right).Should().BeFalse();
        left.Equals(right).Should().BeFalse();
    }
}
