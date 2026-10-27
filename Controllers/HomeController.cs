using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Storefront.Data;
using Storefront.Models;

namespace Storefront.Controllers;

public class HomeController : Controller
{
    private readonly StoreDbContext _storeDb;

    public HomeController(StoreDbContext storeDb)
    {
        _storeDb = storeDb;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _storeDb.Products.ToListAsync();

        return View(products);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
