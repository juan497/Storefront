using Microsoft.EntityFrameworkCore;
using Storefront.Data;
using Storefront.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ====== ADD DATABASE CONTEXT HERE ======
var storeConnection = builder.Configuration.GetConnectionString(
    "StoreConnection")
    ?? throw new InvalidOperationException(
        "The StoreConnection connection string is not configured.");

builder.Services.AddDbContext<StoreDbContext>(
    options => options.UseSqlite(storeConnection));

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

// Configure the HTTP request pipeline.
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
