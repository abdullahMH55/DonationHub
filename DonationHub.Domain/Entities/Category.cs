namespace DonationHub.Domain.Entities;
using DonationHub.Domain.Common;

public class Category : Entity
{ 
    public string NameAr { get; private set; } = string.Empty;
    public string NameEn { get; private set; } = string.Empty;
    public string DescriptionAr { get; private set; } = string.Empty;
    public string DescriptionEn { get; private set; } = string.Empty;
    public DateTime UpdatedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    public Category() { }
    
    private Category(string nameAr, string nameEn, string descriptionAr, string descriptionEn, DateTime time)
    {
        NameAr = nameAr;
        NameEn = nameEn;
        DescriptionAr = descriptionAr;
        DescriptionEn = descriptionEn;
        CreatedAt = time;
        UpdatedAt = time;
    }
    public static Category Create(string nameAr, string nameEn, string descriptionAr, string descriptionEn, DateTime time)
    {
        if (string.IsNullOrWhiteSpace(nameAr) || nameAr.Length > 100)
            throw new ArgumentException("Name cannot be more than 100 characters or empty");
        
        if (string.IsNullOrWhiteSpace(nameEn) || nameEn.Length > 100)
            throw new ArgumentException("Name cannot be more than 100 characters or empty");
        
        return new Category(nameAr, nameEn, descriptionAr, descriptionEn, time);
    }
    
    public void UpdateName(string nameAr, string nameEn, DateTime time)
    {
        if (string.IsNullOrWhiteSpace(nameAr) || nameAr.Length > 100)
            throw new ArgumentException("Name cannot be more than 100 characters or empty");
        
        if (string.IsNullOrWhiteSpace(nameEn) || nameEn.Length > 100)
            throw new ArgumentException("Name cannot be more than 100 characters or empty");
        
        NameAr = nameAr;
        NameEn = nameEn;
        UpdatedAt = time;
    }

    public void UpdateDescription(string descriptionAr, string descriptionEn, DateTime time)
    {
        DescriptionAr = descriptionAr;
        DescriptionEn = descriptionEn;
        UpdatedAt = time;
    }
}
