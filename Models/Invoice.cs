
using System.ComponentModel.DataAnnotations.Schema;

namespace Storefront.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public Order? Order { get; set; }

        public DateTime InvoiceDate { get; set; }

        public string PaymentStatus { get; set; } = "Unpaid";

        [NotMapped]
        public string InvoiceNumber =>
            $"INV-{Id:D4}";
    }
}
