
using Microsoft.AspNetCore.Mvc;
using Storefront.Data;

namespace Storefront.Controllers
{
    public class OrdersController : Controller
    {
        public IActionResult Index()
        {
            return View(StoreData.Orders);
        }

        public IActionResult Details(int id)
        {
            var order = StoreData.Orders
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            var customer = StoreData.Customers
                .FirstOrDefault(c => c.Id == order.CustomerId);

            ViewBag.Customer = customer;

            return View(order);
        }
    }
}
