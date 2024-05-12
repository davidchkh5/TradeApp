using Microsoft.EntityFrameworkCore;
using TradeApp.Data;
using TradeApp.Interfaces;
using TradeApp.Repositories;
using TradeApp.Services;

namespace TradeApp.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<IItemRepository, ItemRepository>();


            return services;
        }
    }
}
