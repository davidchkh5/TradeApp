using TradeApp.Entities;

namespace TradeApp.Interfaces
{
    public interface IUserRepository
    {
        Task<List<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserById(int id);
        Task<AppUser> GetUserByUsernameAsync(string username);
        Task AddUserAsync(AppUser user);
        
        Task UpdateUser(AppUser user);
        Task<bool> SaveChangesAsync();


    }
}
