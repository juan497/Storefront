using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Storefront.Data;
using Storefront.Models;

namespace Storefront.Controllers
{
    public class CartController : Controller
    {
        private const string CartSessionKey = "ShoppingCart";
        private readonly StoreDbContext _storeDb;

        public CartController(StoreDbContext storeDb)
        {
            _storeDb = storeDb;
        }

        private List<CartItem> GetCart()
        {
            var json = HttpContext.Session.GetString(CartSessionKey);

            if (string.IsNullOrEmpty(json))
            {
                return new List<CartItem>();
            }

            return JsonSerializer.Deserialize<List<CartItem>>(json)
                   ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            var json = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CartSessionKey, json);
        }

        public async Task<IActionResult> Index()
        {
            var cart = GetCart();
            var productIds = cart.Select(item => item.ProductId).ToArray();
            var products = await _storeDb.Products
                .Where(product => productIds.Contains(product.Id))
                .ToDictionaryAsync(product => product.Id);

            var cartProducts = cart
                .Select(item =>
                {
                    if (!products.TryGetValue(item.ProductId, out var product))
                    {
                        return null;
                    }

                    return new
                    {
                        Product = product,
                        item.Quantity,
                        Subtotal = product.Price * item.Quantity
                    };
                })
                .Where(item => item != null)
                .ToList();

            ViewBag.CartProducts = cartProducts;
            ViewBag.Total = cartProducts.Sum(item => item!.Subtotal);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId, int quantity = 1)
        {
            var product = await _storeDb.Products
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                return NotFound();
            }

            if (quantity < 1)
            {
                TempData["CartMessage"] = "Quantity must be at least 1.";
                return RedirectToAction("Index", "Products");
            }

            var cart = GetCart();
            var existingItem = cart
                .FirstOrDefault(item => item.ProductId == productId);

            int requestedQuantity =
                (existingItem?.Quantity ?? 0) + quantity;

            if (requestedQuantity > product.StockQuantity)
            {
                TempData["CartMessage"] =
                    $"Only {product.StockQuantity} units of {product.Name} are available.";

                return RedirectToAction("Index", "Products");
            }

            if (existingItem == null)
            {
                cart.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity
                });
            }
            else
            {
                existingItem.Quantity = requestedQuantity;
            }

            SaveCart(cart);

            TempData["CartMessage"] = $"{product.Name} added to your cart.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int productId, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(
                item => item.ProductId == productId);

            if (item == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var product = await _storeDb.Products
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                cart.Remove(item);
            }
            else if (quantity < 1)
            {
                TempData["CartMessage"] =
                    "Quantity must be at least 1. Use Remove to delete an item.";
                return RedirectToAction(nameof(Index));
            }
            else if (quantity > product.StockQuantity)
            {
                TempData["CartMessage"] =
                    $"Only {product.StockQuantity} units of {product.Name} are available.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                item.Quantity = quantity;
            }

            SaveCart(cart);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int productId)
        {
            var cart = GetCart();

            cart.RemoveAll(item => item.ProductId == productId);

            SaveCart(cart);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            if (!GetCart().Any())
            {
                return RedirectToAction(nameof(Index));
            }

            return View(new CheckoutViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var cart = GetCart();

            if (!cart.Any())
            {
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Recheck stock at checkout.
            var productIds = cart.Select(item => item.ProductId).ToArray();
            var products = await _storeDb.Products
                .Where(product => productIds.Contains(product.Id))
                .ToDictionaryAsync(product => product.Id);

            foreach (var item in cart)
            {
                if (!products.TryGetValue(item.ProductId, out var product)
                    || item.Quantity > product.StockQuantity)
                {
                    ModelState.AddModelError(
                        "",
                        $"A product in your cart is no longer available in that quantity.");

                    return View(model);
                }
            }

            // Reuse an existing customer with the same email.
            var email = model.Email.Trim();
            var normalizedEmail = email.ToLower();
            var customer = await _storeDb.Customers
                .FirstOrDefaultAsync(c =>
                    c.Email.ToLower() == normalizedEmail);

            if (customer == null)
            {
                customer = new Customer
                {
                    FirstName = model.FirstName.Trim(),
                    LastName = model.LastName.Trim(),
                    Email = email,
                    City = model.City.Trim()
                };

                _storeDb.Customers.Add(customer);
            }
            else
            {
                // Refresh the existing customer's details.
                customer.FirstName = model.FirstName.Trim();
                customer.LastName = model.LastName.Trim();
                customer.City = model.City.Trim();
            }

            var order = new Order
            {
                CustomerId = customer.Id,
                Customer = customer,
                OrderDate = DateTime.Now,
                Status = "Pending",
                Items = cart.Select(item =>
                {
                    var product = products[item.ProductId];

                    return new OrderItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price
                    };
                }).ToList()
            };

            // Reduce inventory.
            foreach (var item in cart)
            {
                var product = products[item.ProductId];
                product.StockQuantity -= item.Quantity;
            }

            // Create an unpaid invoice; no payment gateway is connected.
            var invoice = new Invoice
            {
                Order = order,
                InvoiceDate = DateTime.Now,
                PaymentStatus = "Unpaid"
            };

            _storeDb.Orders.Add(order);
            _storeDb.Invoices.Add(invoice);
            await _storeDb.SaveChangesAsync();

            // Empty the cart after successful order creation.
            HttpContext.Session.Remove(CartSessionKey);

            return RedirectToAction(
                nameof(Confirmation),
                new { id = order.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Confirmation(int id)
        {
            var order = await _storeDb.Orders
                .Include(order => order.Items)
                .FirstOrDefaultAsync(order => order.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            ViewBag.Customer = await _storeDb.Customers
                .FirstOrDefaultAsync(c => c.Id == order.CustomerId);

            return View(order);
        }
    }
}