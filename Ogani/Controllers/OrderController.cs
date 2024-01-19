using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogani.Data;
using Ogani.Models.Concretes;
using Ogani.ViewModels;
using System.Collections.Generic;

namespace Ogani.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var order = _context.Orders.Include(o => o.OrderModels).ThenInclude(om => om.Product).Include(om => om.User).FirstOrDefault(o => o.User.UserName == User.Identity.Name);
            List<OrderViewModel> products = new();
            if (order != null)
            {
                foreach (var om in order.OrderModels)
                {
                    products.Add(new()
                    {
                        Id = om.Product.Id,
                        ImageUrl = om.Product.ImageUrl,
                        Name = om.Product.Name,
                        Price = om.Product.Price,
                        Quantity = om.Quantity
                    });
                }
            }
            
            return View(products);
        }

        [HttpGet]
        public PartialViewResult UpdateOrder(int productId, int productQuantity)
        {
            var orderModel = _context.OrderModels.Include(om => om.Product).FirstOrDefault(om => om.ProductId == productId);
            orderModel.Quantity = productQuantity;
            _context.OrderModels.Update(orderModel);
            _context.SaveChanges();

            var order = _context.Orders.Include(o => o.OrderModels).ThenInclude(om => om.Product).Include(om => om.User).FirstOrDefault(o => o.User.UserName == User.Identity.Name);
            List<OrderViewModel> products = new();
            if (order != null)
            {
                foreach (var om in order.OrderModels.OrderBy(om => om.Id))
                {
                    products.Add(new()
                    {
                        Id = om.Product.Id,
                        ImageUrl = om.Product.ImageUrl,
                        Name = om.Product.Name,
                        Price = om.Product.Price,
                        Quantity = om.Quantity
                    });
                }
            }

            return PartialView("~/Views/Order/OrdersPartial.cshtml", products); ;
        }

        [HttpGet]
        public PartialViewResult DeleteOrderModel(int orderModelId)
        {
            var orderModel = _context.OrderModels.FirstOrDefault(om => om.ProductId == orderModelId);
            _context.OrderModels.Remove(orderModel);
            _context.SaveChanges();

            var order = _context.Orders.Include(o => o.OrderModels).ThenInclude(om => om.Product).Include(om => om.User).FirstOrDefault(o => o.User.UserName == User.Identity.Name);

            List<OrderViewModel> products = new();
            if (order != null)
            {
                foreach (var om in order.OrderModels.OrderBy(om => om.Id))
                {
                    products.Add(new()
                    {
                        Id = om.ProductId,
                        ImageUrl = om.Product.ImageUrl,
                        Name = om.Product.Name,
                        Price = om.Product.Price,
                        Quantity = om.Quantity
                    });
                }
            }

            if (products.Count > 0)
                return PartialView("~/Views/Order/OrdersPartial.cshtml", products);

            return PartialView("~/Views/Order/EmptyOrderPartial.cshtml");
        }
    }
}
