
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Storefront.Data;

namespace Storefront.Controllers
{
    public class InvoicesController : Controller
    {
        private readonly StoreDbContext _storeDb;

        public InvoicesController(StoreDbContext storeDb)
        {
            _storeDb = storeDb;
        }

        public async Task<IActionResult> Index()
        {
            var invoices = await _storeDb.Invoices
                .Include(invoice => invoice.Order)
                .ThenInclude(order => order!.Items)
                .OrderBy(invoice => invoice.Id)
                .ToListAsync();

            return View(invoices);
        }

        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _storeDb.Invoices
                .Include(invoice => invoice.Order)
                .ThenInclude(order => order!.Items)
                .FirstOrDefaultAsync(invoice => invoice.Id == id);

            if (invoice == null)
            {
                return NotFound();
            }

            var order = invoice.Order;

            if (order == null)
            {
                return NotFound();
            }

            var customer = await _storeDb.Customers
                .FirstOrDefaultAsync(c => c.Id == order.CustomerId);

            ViewBag.Order = order;
            ViewBag.Customer = customer;

            return View(invoice);
        }
    }
}
