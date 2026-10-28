namespace DonationHub.Domain.Entities;

public class Category : Enitiy
{ 
    public string NameAr { get; private set; } = string.Empty;
    public string NameEn { get; private set; } = string.Empty;
    public string DescriptionAr { get; private set; } = string.Empty;
    public string DescriptionEn { get; private set; } = string.Empty;
    public DateTime UpdatedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Category(string nameAr, string nameEn, string descriptionAr, string descriptionEn, DateTime time)
    {
        NameAr  = nameAr;
        NameEn = nameEn;
        DescriptionAr = descriptionAr;
        DescriptionEn = descriptionEn;
        CreatedAt = time;
    }
    
    
    public static Category Create(string nameAr, string nameEn, string descriptionAr, string descriptionEn, DateTime time)
    {
        if(nameAr.Length > 100 || string.IsNullOrEmpty(nameAr))
            throw new ArgumentNullException("nameAr", "Name cannot be more than 100 characters or empty");
        
        if(nameEn.Length > 100 || string.IsNullOrEmpty(nameAr))
            throw new ArgumentNullException("nameEr", "Name cannot be more than 100 characters or empty");
        
        
        var result = new Category(nameAr, nameEn, descriptionAr, descriptionEn, time);
        return result;
    }
    
    public void UpdateName(string nameAr, string nameEn, DateTime time)
    {
        if(nameAr.Length > 100 || string.IsNullOrEmpty(nameAr))
            throw new ArgumentNullException("nameAr", "Name cannot be more than 100 characters or empty");
        
        if(nameEn.Length > 100 || string.IsNullOrEmpty(nameAr))
            throw new ArgumentNullException("nameEr", "Name cannot be more than 100 characters or empty");
        
        this.NameAr =  nameAr;
        this.NameEn = nameEn;
        this.UpdatedAt = time;
    }
    public void UpdateDescription(string descriptionAr, string descriptionEn, DateTime time)
    {
        
        this.DescriptionAr = descriptionAr;
        this.DescriptionEn = descriptionEn;
        this.UpdatedAt = DateTime.UtcNow;
    }
}
