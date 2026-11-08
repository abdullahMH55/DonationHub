using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DonationHub.Infrastructure.Configurations;

public class DonationConfiguration : IEntityTypeConfiguration<Donation>
{
    public void Configure(EntityTypeBuilder<Donation> builder)
    {
        builder.Property(d => d.Amount).HasPrecision(18, 2);
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(d => d.DonorUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Campaign>().WithMany().HasForeignKey(d => d.CampaignId).OnDelete(DeleteBehavior.Restrict);
    }
}
