using Ogani.Models.Concretes;

namespace Ogani.ViewModels
{
    public class ProductUpdateViewModel
    {
        public IFormFile? ImageUrl { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int CategoryId { get; set; }
        public List<Tag> Tags { get; set; }
    }
}
