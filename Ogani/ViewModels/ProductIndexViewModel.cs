using Ogani.Models.Concretes;

namespace Ogani.ViewModels
{
    public class ProductIndexViewModel
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string Category { get; set; }
        public List<Tag> Tags { get; set; }
    }
}
