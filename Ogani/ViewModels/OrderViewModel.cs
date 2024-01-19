using Ogani.Models.Concretes;

namespace Ogani.ViewModels
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}
