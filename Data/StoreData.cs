
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
