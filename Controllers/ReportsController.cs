
using Microsoft.AspNetCore.Mvc;
using Storefront.Data;

namespace Storefront.Controllers
{
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            var paidOrders = StoreData.Orders
                .Where(o => o.Status == "Paid")
                .ToList();

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

            var lowStockProducts = StoreData.Products
                .Where(product => product.StockQuantity <= 10)
                .OrderBy(product => product.StockQuantity)
                .ToList();

            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.PaidOrderCount = paidOrderCount;
            ViewBag.AverageOrderValue = averageOrderValue;
            ViewBag.TopProducts = topProducts;
            ViewBag.LowStockProducts = lowStockProducts;

            return View();
        }
    }
}
