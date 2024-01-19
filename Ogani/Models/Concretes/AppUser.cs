using Microsoft.AspNetCore.Identity;

namespace Ogani.Models.Concretes
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public List<Order> Orders { get; set; }
    }
}
