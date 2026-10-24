
using Storefront.Models;

namespace Storefront.Data
{
    public static class StoreData
    {
public static List<Order> Orders = new()
{
    new Order
    {
        Id = 1001,
        CustomerId = 1,
        OrderDate = new DateTime(2026, 10, 10),
        Status = "Paid",
        Items = new List<OrderItem>
        {
            new OrderItem
            {
                ProductId = 1,
                ProductName = "Wireless Headphones",
                Quantity = 1,
                UnitPrice = 79.99m
            },
            new OrderItem
            {
                ProductId = 4,
                ProductName = "Water Bottle",
                Quantity = 2,
                UnitPrice = 19.99m
            }
        }
    },

    new Order
    {
        Id = 1002,
        CustomerId = 2,
        OrderDate = new DateTime(2026, 10, 12),
        Status = "Pending",
        Items = new List<OrderItem>
        {
            new OrderItem
            {
                ProductId = 2,
                ProductName = "Mechanical Keyboard",
                Quantity = 1,
                UnitPrice = 59.99m
            },
            new OrderItem
            {
                ProductId = 3,
                ProductName = "Running Shoes",
                Quantity = 1,
                UnitPrice = 89.99m
            }
        }
    }
};

public static List<Invoice> Invoices = new()
{
    new Invoice
    {
        Id = 1,
        OrderId = 1001,
        InvoiceDate = new DateTime(2026, 10, 10),
        PaymentStatus = "Paid"
    },

    new Invoice
    {
        Id = 2,
        OrderId = 1002,
        InvoiceDate = new DateTime(2026, 10, 12),
        PaymentStatus = "Unpaid"
    }
};
    }
}
