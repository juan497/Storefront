using Microsoft.EntityFrameworkCore;
using Storefront.Data;
using Storefront.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "DefaultConnection")));

var customerConnection = builder.Configuration.GetConnectionString(
    "CustomerConnection")
    ?? throw new InvalidOperationException(
        "The CustomerConnection connection string is not configured.");

builder.Services.AddDbContext<StoreDbContext>(
    options => options.UseSqlite(customerConnection));

// ====== ADD SESSION SERVICES HERE ======
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => 
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// =======================================

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var storeDb = scope.ServiceProvider
        .GetRequiredService<StoreDbContext>();
    await storeDb.Database.EnsureCreatedAsync();
    await storeDb.Database.ExecuteSqlRawAsync(
        """
        CREATE TABLE IF NOT EXISTS "Products" (
            "Id" INTEGER NOT NULL CONSTRAINT "PK_Products" PRIMARY KEY AUTOINCREMENT,
            "Name" TEXT NOT NULL,
            "Description" TEXT NOT NULL,
            "Price" TEXT NOT NULL,
            "StockQuantity" INTEGER NOT NULL,
            "Category" TEXT NOT NULL
        );
        """);
    await storeDb.Database.ExecuteSqlRawAsync(
        """
        CREATE TABLE IF NOT EXISTS "Orders" (
            "Id" INTEGER NOT NULL CONSTRAINT "PK_Orders" PRIMARY KEY AUTOINCREMENT,
            "CustomerId" INTEGER NOT NULL,
            "OrderDate" TEXT NOT NULL,
            "Status" TEXT NOT NULL,
            CONSTRAINT "FK_Orders_Customers_CustomerId"
                FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id")
                ON DELETE RESTRICT
        );
        CREATE TABLE IF NOT EXISTS "OrderItems" (
            "Id" INTEGER NOT NULL CONSTRAINT "PK_OrderItems" PRIMARY KEY AUTOINCREMENT,
            "OrderId" INTEGER NOT NULL,
            "ProductId" INTEGER NOT NULL,
            "ProductName" TEXT NOT NULL,
            "Quantity" INTEGER NOT NULL,
            "UnitPrice" TEXT NOT NULL,
            CONSTRAINT "FK_OrderItems_Orders_OrderId"
                FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id")
                ON DELETE CASCADE
        );
        CREATE TABLE IF NOT EXISTS "Invoices" (
            "Id" INTEGER NOT NULL CONSTRAINT "PK_Invoices" PRIMARY KEY AUTOINCREMENT,
            "OrderId" INTEGER NOT NULL,
            "InvoiceDate" TEXT NOT NULL,
            "PaymentStatus" TEXT NOT NULL,
            CONSTRAINT "AK_Invoices_OrderId" UNIQUE ("OrderId"),
            CONSTRAINT "FK_Invoices_Orders_OrderId"
                FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id")
                ON DELETE CASCADE
        );
        """);

    if (!await storeDb.Customers.AnyAsync())
    {
        storeDb.Customers.AddRange(
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
            });

        await storeDb.SaveChangesAsync();
    }

    if (!await storeDb.Products.AnyAsync())
    {
        storeDb.Products.AddRange(
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
            });

        await storeDb.SaveChangesAsync();
    }

    if (!await storeDb.Orders.AnyAsync())
    {
        var seededOrders = StoreData.Orders;
        storeDb.Orders.AddRange(seededOrders);

        storeDb.Invoices.AddRange(
            StoreData.Invoices.Select(invoice =>
            {
                invoice.Order = seededOrders.Single(
                    order => order.Id == invoice.OrderId);
                return invoice;
            }));

        await storeDb.SaveChangesAsync();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// ====== ADD SESSION MIDDLEWARE HERE ======
// It must come after UseRouting and before your endpoint mappings
app.UseSession(); 
// =========================================

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
