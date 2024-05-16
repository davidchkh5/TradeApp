using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TradeApp.Entities;
using TradeApp.Interfaces;

namespace TradeApp.Extensions
{
    public class AdminCreateExtension
    {

        public static void CreateAdmin(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {


            var roles = new List<AppRole>
            {
                new AppRole{Name = "Member"},
                new AppRole{Name = "Admin"},
                new AppRole{Name = "Moderator"}
            };

            foreach (var role in roles)
            {

                 roleManager.CreateAsync(role);
            }

            if (!userManager.Users.Any(u => u.UserName == "admin"))
            {
                var admin = new AppUser
                {
                    UserName = "admin"
                };
                 userManager.CreateAsync(admin, "Pa$$w0rd");
                 userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
            }
        }

    }
}
