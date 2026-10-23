
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Storefront.Data;

namespace Storefront.Controllers
{
    public class InvoicesController : Controller
    {
        private readonly CustomerDbContext _customerDb;

        public InvoicesController(CustomerDbContext customerDb)
        {
            _customerDb = customerDb;
        }

        public IActionResult Index()
        {
            return View(StoreData.Invoices);
        }

        public async Task<IActionResult> Details(int id)
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

            var customer = await _customerDb.Customers
                .FirstOrDefaultAsync(c => c.Id == order.CustomerId);

            ViewBag.Order = order;
            ViewBag.Customer = customer;

            return View(invoice);
        }
    }
}
