using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DonationHub.Infrastructure.Configurations;

public class CampaignConfig : IEntityTypeConfiguration<Campaign>
{
    public void Configure(EntityTypeBuilder<Campaign> builder)
    {
        builder.Property(c => c.RowVersion).IsRowVersion();
        builder.Property(c => c.TitleAr).HasMaxLength(200).IsRequired();
        builder.Property(c => c.TitleEn).HasMaxLength(200).IsRequired();
        builder.Property(c => c.DescriptionAr).HasMaxLength(2000);
        builder.Property(c => c.DescriptionEn).HasMaxLength(2000);
        builder.Property(c => c.BannerFileName).HasMaxLength(255);
        builder.Property(c => c.GoalAmount).HasPrecision(18, 2);
        builder.Property(c => c.RaisedAmount).HasPrecision(18, 2);
        builder.HasOne(c => c.Category).WithMany().HasForeignKey(c => c.CategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(c => c.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(c => c.Status);
        
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
