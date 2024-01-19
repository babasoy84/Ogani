using Ogani.Models.Abstracts;

namespace Ogani.Models.Concretes
{
    public class Product : Entity
    {
        public int CategoryId { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public Category Category { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
        public List<OrderModel> OrderModels { get; set; }

    }
}
