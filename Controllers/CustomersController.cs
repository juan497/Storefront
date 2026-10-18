
using Microsoft.AspNetCore.Mvc;
using Storefront.Data;

namespace Storefront.Controllers
{
    public class CustomersController : Controller
    {
        public IActionResult Index()
        {
            var customers = StoreData.Customers;

            return View(customers);
        }

        public IActionResult Details(int id)
        {
            var customer = StoreData.Customers
                .FirstOrDefault(c => c.Id == id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }
    }
}
