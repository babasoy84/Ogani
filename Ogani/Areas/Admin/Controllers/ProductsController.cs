using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using Ogani.Data;
using Ogani.Models.Concretes;
using Ogani.ViewModels;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;

namespace Ogani.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Moderator")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly Account cloudinaryAccount;
        private readonly Cloudinary cloudinary;

        public ProductsController(AppDbContext dbContext, IWebHostEnvironment hostEnvironment, IConfiguration configuration)
        {
            _dbContext = dbContext;
            webHostEnvironment = hostEnvironment;
            cloudinaryAccount = new Account("dv9hubcxy", "956258466281318", "wG5We44Sc-9SThiN68YKgZ8HPbY");
            cloudinary = new Cloudinary(cloudinaryAccount);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<ProductIndexViewModel> products = new();
            foreach (var product in _dbContext.Products.Include("Category").Include("Tags"))
            {
                products.Add(new ProductIndexViewModel()
                {
                    Id = product.Id,
                    ImageUrl = product.ImageUrl,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Category = product.Category.Name,
                    Tags = product.Tags.ToList()
                });
            }

            return View(products);
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Categories = new SelectList(_dbContext.Categories, "Id", "Name");
            ViewBag.Tags = new MultiSelectList(_dbContext.Tags, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(ProductAddViewModel model, int[] tags)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_dbContext.Categories, "Id", "Name");
                ViewBag.Tags = new MultiSelectList(_dbContext.Tags, "Id", "Name", tags);
                return View(model);
            }

            List<Tag> Tags = new();

            foreach (var t in tags)
            {
                Tags.Add(_dbContext.Tags.FirstOrDefault(x => x.Id == t));
            }

            var path = UploadFile(model.ImageUrl);

            var product = new Product()
            {
                ImageUrl = path,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                CategoryId = model.CategoryId,
                Tags = Tags
            };

            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int Id)
        {
            var product = await _dbContext.Products.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == Id);
            ViewBag.Categories = new SelectList(_dbContext.Categories, "Id", "Name");
            ViewBag.Tags = new MultiSelectList(_dbContext.Tags, "Id", "Name", product.Tags.Select(t => t.Id));

            var model = new ProductUpdateViewModel
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int Id, ProductUpdateViewModel model, int[] tags)
        {
            var product = await _dbContext.Products.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == Id);

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_dbContext.Categories, "Id", "Name");
                ViewBag.Tags = new MultiSelectList(_dbContext.Tags, "Id", "Name", tags);

                return View(model);
            }

            List<Tag> Tags = new();

            foreach (var t in tags)
            {
                Tags.Add(_dbContext.Tags.FirstOrDefault(x => x.Id == t));
            }

            string path;
            if (model.ImageUrl == null)
                path = product.ImageUrl;
            else
            {
                System.IO.File.Delete(Path.Combine(Path.Combine(webHostEnvironment.WebRootPath, "images"), product.ImageUrl));
                path = UploadFile(model.ImageUrl);
            }

            product.ImageUrl = path;
            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.CategoryId = model.CategoryId;
            product.Tags = Tags;

            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int Id)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == Id);

            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        private string UploadFile(IFormFile file)
        {
            string uniqueFileName = null;

            if (file == null || file.Length == 0)
            {
                return null;
            }

            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream)
                };

                uniqueFileName = cloudinary.Upload(uploadParams).Uri.AbsoluteUri;
            }


            return uniqueFileName;
        }
    }
}
