using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using Ogani.Data;
using Ogani.Models.Concretes;
using Ogani.ViewModels;
using PagedList;
using System.Collections.Generic;

namespace Ogani.Controllers
{
    public class ShopController : Controller
    {
        private AppDbContext _context;

        public ShopController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]                                
        public IActionResult Index()
        {
            List<ShopViewModel> models = new List<ShopViewModel>();

            foreach (var product in _context.Products.OrderBy(p => p.Price).ToList())
            {
                models.Add(new ShopViewModel { Id = product.Id, ImageUrl = product.ImageUrl, Name = product.Name, Price = product.Price });
            }

            ViewBag.Categories = _context.Categories.ToList();

            if (models.Count > 0)
            {
				ViewBag.FoundProductsCount = models.Count;
				ViewBag.MaxPrice = models.OrderByDescending(p => p.Price).Select(p => p.Price).First();
				ViewBag.MinPrice = models.OrderBy(p => p.Price).Select(p => p.Price).First();
				ViewBag.SortOption = 1;
				var list = models.ToPagedList(1, 3);
				ViewBag.PageNumber = 1;
				ViewBag.PageCount = list.PageCount;
				return View(list);
			}


			return View(null);
		}

        [HttpGet]
        public PartialViewResult ProductsPartial(List<string> selectedCategories, string minPrice, string maxPrice, int? page, int sortOption, string searchText)
        {
            List<ShopViewModel> models = new List<ShopViewModel>();
            var products = new List<Product>();

            if (searchText != null)
            {
                foreach (var product in _context.Products.Include(p => p.Tags).ToList())
                {
                    bool b = false;
                    foreach (var tag in product.Tags)
                    {
                        if (searchText.ToUpper() == tag.Name.ToUpper())
                        {
                            b = true;
                            break;
                        }

                    }
                    if (b || product.Name.ToUpper().Contains(searchText.ToUpper()))
                    {
                        products.Add(product);
                    }
                }
            }
            else
            {
                foreach (var product in _context.Products.ToList())
                {
                    products.Add(product);
                }
            }
            if (products?.Count == 0)
                return PartialView("~/Views/Shop/NotFoundPartial.cshtml");
            else if (selectedCategories[0] != null && selectedCategories[0].Split(',')?.Length > 0)
            {
                foreach (var product in products)
                {
                    foreach (string categoryId in selectedCategories[0].Split(','))
                    {
                        if (product.CategoryId.ToString() == categoryId)
                        {
                            models.Add(new ShopViewModel { Id = product.Id, ImageUrl = product.ImageUrl, Name = product.Name, Price = product.Price });
                            break;
                        }
                    }
                }
            }
            else
            {
                foreach (var product in products)
                {
                    models.Add(new ShopViewModel { Id = product.Id, ImageUrl = product.ImageUrl, Name = product.Name, Price = product.Price });
                }
            }

            models = models.Where(p => p.Price >= int.Parse(minPrice.Replace("$", "")) && p.Price <= int.Parse(maxPrice.Replace("$", ""))).ToList();
            

            if (models == null || models?.Count == 0)
                return PartialView("~/Views/Shop/NotFoundPartial.cshtml");

            ViewBag.MaxPrice = models?.OrderByDescending(p => p.Price).Select(p => p.Price).First();
            ViewBag.MinPrice = models?.OrderBy(p => p.Price).Select(p => p.Price).First();

            switch (sortOption)
            {
                case 1:
                    models = models.OrderBy(p => p.Price).ToList();
                    break;
                case 2:
                    models = models.OrderByDescending(p => p.Price).ToList();
                    break;
                default:
                    break;
            }

            ViewBag.FoundProductsCount = models.Count;
            ViewBag.SelectedCategories = selectedCategories;
            ViewBag.SortOption = sortOption;
            ViewBag.SelectedMaxPrice = maxPrice;
            ViewBag.SelectedMinPrice = minPrice;
            int pageNumber = (page ?? 1);
            var list = models.ToPagedList(pageNumber, 3);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageCount = list.PageCount;
            return PartialView("~/Views/Shop/ProductsPartial.cshtml", list);
        }

        [HttpGet]
        public PartialViewResult AddOrder(List<string> selectedCategories, string minPrice, string maxPrice, int? page, int sortOption, string searchText, int productId)
        {
            var order = _context.Orders.Include(o => o.OrderModels).Include(o => o.User).FirstOrDefault(o => o.User.UserName == User.Identity.Name);
            if (order == null)
            {
                order = new();
                order.UserId = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name)?.Id;
                _context.Orders.Add(order);
                _context.SaveChanges();
            }

            if (!order.OrderModels.Select(om => om.ProductId).Contains(productId))
            {
                var om = new OrderModel
                {
                    ProductId = _context.Products.FirstOrDefault(p => p.Id == productId).Id,
                    OrderId = order.Id,
                    Quantity = 1
                };

                _context.OrderModels.Add(om);
                _context.SaveChanges();

                if (order.OrderModels == null)
                    order.OrderModels = new();

                order.OrderModels.Add(om);
                _context.Update(order);
                _context.SaveChanges();
            }


            return ProductsPartial(selectedCategories, minPrice, maxPrice, page, sortOption, searchText);
        }
    }
}
