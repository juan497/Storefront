
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Storefront.Data;

namespace Storefront.Controllers
{
    public class ProductsController : Controller
    {
        private readonly StoreDbContext _storeDb;

        public ProductsController(StoreDbContext storeDb)
        {
            _storeDb = storeDb;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _storeDb.Products.ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _storeDb.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}
