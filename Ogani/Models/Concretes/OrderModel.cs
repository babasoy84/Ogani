using Ogani.Models.Abstracts;

namespace Ogani.Models.Concretes
{
    public class OrderModel : Entity
    {
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public Product Product { get; set; }
        public Order Order { get; set; }
        public int Quantity { get; set; }
    }
}
