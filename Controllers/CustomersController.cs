
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Storefront.Data;

namespace Storefront.Controllers
{
    public class CustomersController : Controller
    {
        private readonly StoreDbContext _storeDb;

        public CustomersController(StoreDbContext storeDb)
        {
            _storeDb = storeDb;
        }
        
        public async Task<IActionResult> Index()
        {
            var customers = await _storeDb.Customers.ToListAsync();

            return View(customers);
        }

        public async Task<IActionResult> Details(int id)
        {
            var customer = await _storeDb.Customers
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }
    }
}
