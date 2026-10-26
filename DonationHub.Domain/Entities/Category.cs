namespace DonationHub.Domain.Entities;

public class Category : CommonData
{ 
    public string NameAr { get; private set; }
    public string NameEn  { get; private set; }
    public string DescriptionAr { get; private set;}
    public string DescriptionEn {get; private set;}
    public DateTime UpdatedAt  { get; private set;}
}