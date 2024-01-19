using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ogani.Data;
using Ogani.Models.Concretes;
using Ogani.ViewModels;

namespace Ogani.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Moderator")]
    public class CategoriesController : Controller
    {
        private readonly AppDbContext _dbContext;

        public CategoriesController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<CategoryIndexViewModel> categories = new();
            foreach (var category in _dbContext.Categories.Include("Products"))
            {
                categories.Add(new CategoryIndexViewModel()
                {
                    Id = category.Id,
                    Name = category.Name,
                    ProductCount = category.Products.Count()
                });
            }

            return View(categories);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(CategoryAddViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var category = new Category()
            {
                Name = model.Name
            };

            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int Id)
        {
            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == Id);

            var model = new CategoryAddViewModel
            {
                Name = category.Name
            };

            ViewBag.CategoryId = category.Id;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int Id, CategoryAddViewModel model)
        {
            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == Id);

            if (!ModelState.IsValid)
                return View(model);

            category.Name = model.Name;

            _dbContext.Categories.Update(category);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int Id)
        {
            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == Id);

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
