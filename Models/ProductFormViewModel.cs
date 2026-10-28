using System.ComponentModel.DataAnnotations;

namespace Storefront.Models;

public class ProductFormViewModel
{
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, 1000000)]
    public decimal Price { get; set; }

    [Range(0, 1000000)]
    public int StockQuantity { get; set; }

    [Required]
    [StringLength(100)]
    public string Category { get; set; } = string.Empty;

    public string? ExistingImageUrl { get; set; }

    public IFormFile? ImageFile { get; set; }
}
