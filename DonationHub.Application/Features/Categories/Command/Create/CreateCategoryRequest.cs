using System.ComponentModel.DataAnnotations;

namespace DonationHub.Application.Features.Categories.Command.Create;

public class CreateCategoryRequest
{

    public string NameAr { get; set; } = string.Empty;
    
    public string NameEn { get; set; } = string.Empty;

    public string? DescriptionAr { get; set; }

    public string? DescriptionEn { get; set; }
}
