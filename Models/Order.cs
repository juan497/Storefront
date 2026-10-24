
namespace Storefront.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public Customer? Customer { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; } = "Pending";

        public List<OrderItem> Items { get; set; } = new();

        public Invoice? Invoice { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public decimal Total => Items.Sum(item => item.Subtotal);
    }
}
