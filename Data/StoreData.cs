
using Storefront.Models;

namespace Storefront.Data
{
    public static class StoreData
    {
        public static List<Product> Products = new()
        {
            new Product
            {
                Id = 1,
                Name = "Wireless Headphones",
                Description = "Bluetooth headphones with clear sound.",
                Price = 79.99m,
                StockQuantity = 25,
                Category = "Electronics"
            },

            new Product
            {
                Id = 2,
                Name = "Mechanical Keyboard",
                Description = "Mechanical keyboard for work and gaming.",
                Price = 59.99m,
                StockQuantity = 15,
                Category = "Electronics"
            },

            new Product
            {
                Id = 3,
                Name = "Running Shoes",
                Description = "Lightweight shoes for everyday running.",
                Price = 89.99m,
                StockQuantity = 30,
                Category = "Clothing"
            },

            new Product
            {
                Id = 4,
                Name = "Water Bottle",
                Description = "Reusable stainless steel water bottle.",
                Price = 19.99m,
                StockQuantity = 50,
                Category = "Accessories"
            }
        };


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



        public static List<Customer> Customers = new()
        {
            new Customer
            {
                Id = 1,
                FirstName = "John",
                LastName = "Smith",
                Email = "john@example.com",
                City = "Los Angeles"
            },

            new Customer
            {
                Id = 2,
                FirstName = "Maria",
                LastName = "Garcia",
                Email = "maria@example.com",
                City = "Huntington Park"
            },

            new Customer
            {
                Id = 3,
                FirstName = "David",
                LastName = "Lee",
                Email = "david@example.com",
                City = "Long Beach"
            }
        };
    }
}
