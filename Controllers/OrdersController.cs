
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Storefront.Data;

namespace Storefront.Controllers
{
    public class OrdersController : Controller
    {
        private readonly StoreDbContext _storeDb;

        public OrdersController(StoreDbContext storeDb)
        {
            _storeDb = storeDb;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.CustomerNames = await _storeDb.Customers
                .ToDictionaryAsync(
                    customer => customer.Id,
                    customer => $"{customer.FirstName} {customer.LastName}");

            var orders = await _storeDb.Orders
                .Include(order => order.Items)
                .OrderBy(order => order.Id)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _storeDb.Orders
                .Include(order => order.Items)
                .Include(order => order.Customer)
                .FirstOrDefaultAsync(order => order.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            ViewBag.Customer = order.Customer;

            return View(order);
        }
    }
}
