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

builder.Services.AddDbContext<CustomerDbContext>(
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
    var customerDb = scope.ServiceProvider
        .GetRequiredService<CustomerDbContext>();
    await customerDb.Database.EnsureCreatedAsync();

    if (!await customerDb.Customers.AnyAsync())
    {
        customerDb.Customers.AddRange(
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

        await customerDb.SaveChangesAsync();
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
