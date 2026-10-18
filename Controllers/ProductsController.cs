
using Microsoft.AspNetCore.Mvc;
using Storefront.Data;

namespace Storefront.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            var products = StoreData.Products;

            return View(products);
        }

        public IActionResult Details(int id)
        {
            var product = StoreData.Products
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}
