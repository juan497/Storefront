
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Storefront.Data;

namespace Storefront.Controllers
{
    public class OrdersController : Controller
    {
        private readonly CustomerDbContext _customerDb;

        public OrdersController(CustomerDbContext customerDb)
        {
            _customerDb = customerDb;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.CustomerNames = await _customerDb.Customers
                .ToDictionaryAsync(
                    customer => customer.Id,
                    customer => $"{customer.FirstName} {customer.LastName}");

            return View(StoreData.Orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = StoreData.Orders
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            var customer = await _customerDb.Customers
                .FirstOrDefaultAsync(c => c.Id == order.CustomerId);

            ViewBag.Customer = customer;

            return View(order);
        }
    }
}
