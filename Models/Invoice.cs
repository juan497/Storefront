
namespace Storefront.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public DateTime InvoiceDate { get; set; }

        public string PaymentStatus { get; set; } = "Unpaid";

        public string InvoiceNumber =>
            $"INV-{Id:D4}";
    }
}
