using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Storefront.Data;
using Storefront.Models;

// <summary>
// The main entry point for the application.
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure database connections for StoreDbContext and ApplicationDbContext
var storeConnection = builder.Configuration.GetConnectionString(
    "StoreConnection")
    ?? throw new InvalidOperationException(
        "The StoreConnection connection string is not configured.");

var identityConnection = builder.Configuration.GetConnectionString(
    "IdentityConnection")
    ?? throw new InvalidOperationException(
        "The IdentityConnection connection string is not configured.");

// Configure Entity Framework Core to use SQLite for both StoreDbContext and ApplicationDbContext
builder.Services.AddDbContext<StoreDbContext>(
    options => options.UseSqlite(storeConnection));
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlite(identityConnection));

// Configure Identity services with custom password requirements
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Configure session state with a 30-minute idle timeout and essential cookies
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Build the application
var app = builder.Build();

// Ensure the databases are created and seed the Identity database with a default admin user
await using (var scope = app.Services.CreateAsyncScope())
{
    var storeDb = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
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

    var identityDb = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await identityDb.Database.EnsureCreatedAsync();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    const string adminEmail = "admin@storefront.com";
    if (await userManager.FindByEmailAsync(adminEmail) is null)
    {
        var adminUser = new IdentityUser(adminEmail)
        {
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, "Password123!");
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                "Failed to create default Identity admin user: " + string.Join(", ", result.Errors.Select(error => error.Description)));
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
