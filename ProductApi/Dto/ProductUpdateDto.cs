using System.ComponentModel.DataAnnotations;

namespace ProductApi.Dto;

public class ProductUpdateDto
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Name { get; set; } = null!;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
}
