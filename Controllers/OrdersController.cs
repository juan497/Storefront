
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Storefront.Data;
    using Storefront.Models;

    namespace Storefront.Controllers
    {
        [Authorize]
        public class OrdersController : Controller
    {
        private readonly StoreDbContext _storeDb;

        public OrdersController(StoreDbContext storeDb)
        {
            _storeDb = storeDb;
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var customerEmail = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(customerEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = await _storeDb.Customers
                .FirstOrDefaultAsync(c => c.Email.ToLower() == customerEmail.ToLower());

            if (customer == null)
            {
                return View(new List<Order>());
            }

            var orders = await _storeDb.Orders
                .Where(order => order.CustomerId == customer.Id)
                .Include(order => order.Items)
                .OrderByDescending(order => order.OrderDate)
                .ToListAsync();

            ViewBag.CustomerNames = new Dictionary<int, string>
            {
                [customer.Id] = $"{customer.FirstName} {customer.LastName}".Trim()
            };

            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var customerEmail = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(customerEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = await _storeDb.Customers
                .FirstOrDefaultAsync(c => c.Email.ToLower() == customerEmail.ToLower());

            if (customer == null)
            {
                return Forbid();
            }

            var order = await _storeDb.Orders
                .Include(order => order.Items)
                .Include(order => order.Customer)
                .FirstOrDefaultAsync(order => order.Id == id && order.CustomerId == customer.Id);

            if (order == null)
            {
                return NotFound();
            }

            ViewBag.Customer = order.Customer;

            return View(order);
        }
    }
}
