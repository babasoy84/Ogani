using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogani.Data;
using Ogani.Models.Concretes;
using Ogani.ViewModels;

namespace Ogani.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Moderator")]
    public class TagsController : Controller
    {
        private readonly AppDbContext _dbContext;

        public TagsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<TagIndexViewModel> tags = new();

            foreach (var tag in _dbContext.Tags.Include("Products"))
            {
                tags.Add(new TagIndexViewModel()
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    ProductCount = tag.Products.Count()
                });
            }

            return View(tags);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(TagAddViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _dbContext.Tags.Add(new Tag() { Name = model.Name });
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int Id)
        {
            var tag = await _dbContext.Tags.FirstOrDefaultAsync(t => t.Id == Id);

            var model = new CategoryAddViewModel
            {
                Name = tag.Name
            };

            ViewBag.TagId = tag.Id;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int Id, TagAddViewModel model)
        {
            var tag = await _dbContext.Tags.FirstOrDefaultAsync(t => t.Id == Id);

            if (!ModelState.IsValid)
                return View(model);

            tag.Name = model.Name;

            _dbContext.Tags.Update(tag);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int Id)
        {
            var tag = await _dbContext.Tags.FirstOrDefaultAsync(t => t.Id == Id);

            _dbContext.Tags.Remove(tag);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
