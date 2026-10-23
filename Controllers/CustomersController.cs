
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Storefront.Data;

namespace Storefront.Controllers
{
    public class CustomersController : Controller
    {
        private readonly CustomerDbContext _customerDb;

        public CustomersController(CustomerDbContext customerDb)
        {
            _customerDb = customerDb;
        }

        public async Task<IActionResult> Index()
        {
            var customers = await _customerDb.Customers.ToListAsync();

            return View(customers);
        }

        public async Task<IActionResult> Details(int id)
        {
            var customer = await _customerDb.Customers
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }
    }
}
