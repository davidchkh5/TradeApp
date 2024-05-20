using Microsoft.EntityFrameworkCore;
using TradeApp.Data;
using TradeApp.Helpers;
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
                opt.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            });
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<IItemRepository, ItemRepository>();
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddScoped<IItemPhotoService, ItemPhotoService>();
           


            return services;
        }
    }
}
