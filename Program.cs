using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Storefront.Data;
using Storefront.Models;

// <summary>
// The main entry point for the application.
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure database provider and connection strings for StoreDbContext and ApplicationDbContext.
var databaseProvider = builder.Configuration["DatabaseProvider"] ?? "PostgreSQL";

builder.Services.AddDbContext<StoreDbContext>((_, options) =>
{
    var connectionString = builder.Configuration.GetConnectionString("StoreConnection")
        ?? throw new InvalidOperationException(
            "The StoreConnection connection string is not configured.");

    ConfigureDatabaseProvider(options, databaseProvider, connectionString);
});

builder.Services.AddDbContext<ApplicationDbContext>((_, options) =>
{
    var connectionString = builder.Configuration.GetConnectionString("IdentityConnection")
        ?? throw new InvalidOperationException(
            "The IdentityConnection connection string is not configured.");

    ConfigureDatabaseProvider(options, builder.Configuration["IdentityDatabaseProvider"] ?? databaseProvider, connectionString);
});

static void ConfigureDatabaseProvider(DbContextOptionsBuilder options, string databaseProvider, string connectionString)
{
    switch (databaseProvider.Trim())
    {
        case "PostgreSQL":
        case "Postgres":
        case "Npgsql":
            options.UseNpgsql(connectionString);
            break;
        case "SqlServer":
        case "SQLServer":
        case "AzureSQL":
        case "AzureSql":
            options.UseSqlServer(connectionString);
            break;
        default:
            throw new InvalidOperationException(
                $"Unsupported database provider '{databaseProvider}'. Use PostgreSQL or SqlServer.");
    }
}

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
    .AddRoles<IdentityRole>()
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

// Ensure the databases are created and seed the Identity database with a default admin user.
await using (var scope = app.Services.CreateAsyncScope())
{
    var storeDb = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
    await storeDb.Database.EnsureCreatedAsync();

    var identityDb = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await identityDb.Database.EnsureCreatedAsync();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    foreach (var roleName in new[] { "Admin", "Customer" })
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    var adminEmail = builder.Configuration["Admin:Email"];
    var adminPassword = builder.Configuration["Admin:Password"];

    if (!string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPassword))
    {
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser is null)
        {
            adminUser = new IdentityUser(adminEmail)
            {
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    "Failed to create configured admin user: " + string.Join(", ", result.Errors.Select(error => error.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            var roleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Failed to assign admin role: " + string.Join(", ", roleResult.Errors.Select(error => error.Description)));
            }
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
