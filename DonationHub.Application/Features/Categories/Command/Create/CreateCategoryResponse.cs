namespace DonationHub.Application.Features.Categories.DTOs;

public class CreateCategoryResponse
{
    public Guid Id { get; set; }
    public string NameAr { get; set; }
    public string NameEn { get; set; }
    public string DescriptionAr { get; set; }
    public string DescriptionEn { get; set; }
}
