using DonationHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DonationHub.Infrastructure.Configurations;

public class CategoryConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.Property(c => c.NameAr).HasMaxLength(100).IsRequired();
        builder.Property(c => c.NameEn).HasMaxLength(100).IsRequired();
        
        builder.HasIndex(c =>  c.NameEn).IsUnique();
        builder.HasIndex(c =>  c.NameAr).IsUnique();
        
    }
}