using Microsoft.EntityFrameworkCore;
using TradeApp.Data;
using TradeApp.Entities;
using TradeApp.Interfaces;

namespace TradeApp.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context; 
        }

        public async Task AddUserAsync(AppUser user)
        {
           await _context.Users.AddAsync(user);
        }

        public async Task<AppUser> GetUserById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.Include(u=> u.Items).FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<List<AppUser>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }
        
        

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;  
        }

        public Task UpdateUser(AppUser user)
        {
            _context.Users.Update(user);
            return Task.CompletedTask;
        }
    }
}
