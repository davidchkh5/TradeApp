using TradeApp.Entities;

namespace TradeApp.Interfaces
{
    public interface IUserRepository
    {
        Task<List<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserById(int id);
        Task<AppUser> GetUserByUsername(string username);
        Task AddUserAsync(AppUser user);
        Task SaveChangesAsync();


    }
}
