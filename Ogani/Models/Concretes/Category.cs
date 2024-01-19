using Ogani.Models.Abstracts;

namespace Ogani.Models.Concretes
{
    public class Category : Entity
    {
        public string Name { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
