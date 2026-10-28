
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Storefront.Data;

    namespace Storefront.Controllers
    {
        [Authorize(Roles = "Admin")]
        public class ReportsController : Controller
    {
        private readonly StoreDbContext _storeDb;

        public ReportsController(StoreDbContext storeDb)
        {
            _storeDb = storeDb;
        }

        public async Task<IActionResult> Index()
        {
            var paidOrders = await _storeDb.Orders
                .Where(o => o.Status == "Paid")
                .Include(order => order.Items)
                .ToListAsync();

            decimal totalRevenue = paidOrders
                .Sum(o => o.Total);

            int paidOrderCount = paidOrders.Count;

            decimal averageOrderValue = paidOrderCount > 0
                ? totalRevenue / paidOrderCount
                : 0m;

            var topProducts = paidOrders
                .SelectMany(o => o.Items)
                .GroupBy(item => item.ProductName)
                .Select(group => new
                {
                    ProductName = group.Key,
                    UnitsSold = group.Sum(item => item.Quantity),
                    Revenue = group.Sum(item => item.Subtotal)
                })
                .OrderByDescending(product => product.UnitsSold)
                .ToList();

            var lowStockProducts = await _storeDb.Products
                .Where(product => product.StockQuantity <= 10)
                .OrderBy(product => product.StockQuantity)
                .ToListAsync();

            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.PaidOrderCount = paidOrderCount;
            ViewBag.AverageOrderValue = averageOrderValue;
            ViewBag.TopProducts = topProducts;
            ViewBag.LowStockProducts = lowStockProducts;

            return View();
        }
    }
}
