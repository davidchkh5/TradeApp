using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TradeApp.Entities;
using TradeApp.Interfaces;

namespace TradeApp.Extensions
{
    public class AdminCreateExtension
    {

        public async static Task CreateAdmin(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {


            var roles = new List<AppRole>
            {
                new AppRole{Name = "Member"},
                new AppRole{Name = "Admin"},
                new AppRole{Name = "Moderator"}
            };

            foreach (var role in roles)
            {

                 await roleManager.CreateAsync(role);
            }

            if (!userManager.Users.Any(u => u.UserName == "admin"))
            {
                var admin = new AppUser
                {
                    UserName = "admin"
                };
                await userManager.CreateAsync(admin, "Pa$$w0rd");
                await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
            }
        }

    }
}
