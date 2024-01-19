using Ogani.Models.Abstracts;

namespace Ogani.Models.Concretes
{
    public class Tag : Entity
    {
        public string Name { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
