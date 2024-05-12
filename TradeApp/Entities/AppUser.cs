using Microsoft.AspNetCore.Identity;

namespace TradeApp.Entities
{
    public class AppUser : IdentityUser<int>
    {   
        public string Gender { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public ICollection<AppUserRole> UserRoles { get; set; }
        public List<Item> Items { get; set; }

    }
}
