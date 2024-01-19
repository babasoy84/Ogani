using Ogani.Models.Abstracts;

namespace Ogani.Models.Concretes
{
    public class Order : Entity
    {
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public List<OrderModel> OrderModels { get; set; }
    }
}
