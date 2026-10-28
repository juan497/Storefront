
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Storefront.Data;
using Storefront.Models;

namespace Storefront.Controllers
{
    public class ProductsController : Controller
    {
        private readonly StoreDbContext _storeDb;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(StoreDbContext storeDb, IWebHostEnvironment webHostEnvironment)
        {
            _storeDb = storeDb;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _storeDb.Products.ToListAsync();

            return View(products);
        }

        public IActionResult Create()
        {
            return View(new ProductFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                StockQuantity = model.StockQuantity,
                Category = model.Category,
                ImageUrl = await SaveImageAsync(model.ImageFile)
            };

            _storeDb.Products.Add(product);
            await _storeDb.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _storeDb.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        private async Task<string?> SaveImageAsync(IFormFile? imageFile)
        {
            if (imageFile is null || imageFile.Length == 0)
            {
                return "/images/no-image.svg";
            }

            var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "images");
            Directory.CreateDirectory(uploadFolder);

            var extension = Path.GetExtension(imageFile.FileName);
            var fileName = $"{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(uploadFolder, fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"/images/{fileName}";
        }
    }
}
