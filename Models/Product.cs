
// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;

// namespace Storefront.Models;

// public class Product
// {
//     public int Id { get; set; }

//     [Required]
//     [StringLength(150)]
//     public string Name { get; set; } = string.Empty;

//     [StringLength(2000)]
//     public string Description { get; set; } = string.Empty;

//     [Column(TypeName = "decimal(18,2)")]
//     [Range(0.01, 1000000)]
//     public decimal Price { get; set; }

//     [Range(0, 1000000)]
//     public int StockQuantity { get; set; }

//     [StringLength(500)]
//     public string? ImageUrl { get; set; }

//     public DateTime CreatedAt { get; set; } =
//         DateTime.UtcNow;
// }



namespace Storefront.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public string Category { get; set; } = "";

        public string? ImageUrl { get; set; }
    }
}
