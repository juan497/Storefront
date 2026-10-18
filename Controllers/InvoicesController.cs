
using Microsoft.AspNetCore.Mvc;
using Storefront.Data;

namespace Storefront.Controllers
{
    public class InvoicesController : Controller
    {
        public IActionResult Index()
        {
            return View(StoreData.Invoices);
        }

        public IActionResult Details(int id)
        {
            var invoice = StoreData.Invoices
                .FirstOrDefault(i => i.Id == id);

            if (invoice == null)
            {
                return NotFound();
            }

            var order = StoreData.Orders
                .FirstOrDefault(o => o.Id == invoice.OrderId);

            if (order == null)
            {
                return NotFound();
            }

            var customer = StoreData.Customers
                .FirstOrDefault(c => c.Id == order.CustomerId);

            ViewBag.Order = order;
            ViewBag.Customer = customer;

            return View(invoice);
        }
    }
}
