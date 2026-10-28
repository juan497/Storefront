
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

        //Task<IActionResult> is Asynchronous (Non-blocking), IActionResult is synchronous (Blocking)
        public async Task<IActionResult> Index()///http://localhost:5041/Customers
        {
            var customers = await _storeDb.Customers.ToListAsync();

            return View(customers);
        }


        //http://localhost:5041/Customers/Details/4 this is an endpoint
        public async Task<IActionResult> Details(int id)
        {
            var customer = await _storeDb.Customers
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                return NotFound();
            }

            var customerOrders = await _storeDb.Orders
                .Where(order => order.CustomerId == customer.Id)
                .Include(order => order.Items)
                .OrderByDescending(order => order.OrderDate)
                .ToListAsync();

            ViewBag.CustomerOrders = customerOrders;

            return View(customer);
        }
    }
}
