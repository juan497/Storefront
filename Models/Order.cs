
namespace Storefront.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; } = "Pending";

        public List<OrderItem> Items { get; set; } = new();

        public decimal Total
        {
            get
            {
                return Items.Sum(item =>
                    item.UnitPrice * item.Quantity);
            }
        }
    }
}
