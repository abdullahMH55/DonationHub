using System.ComponentModel.DataAnnotations;

namespace DonationHub.Application.Features.Categories.Create;

public class CreateCategoryRequest
{
    [Required]
    [MaxLength(100)]
    public string NameAr { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string NameEn { get; set; } = string.Empty;

    public string? DescriptionAr { get; set; }

    public string? DescriptionEn { get; set; }
}
