﻿using Microsoft.EntityFrameworkCore;
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

        public async Task<AppUser> GetUserByUsername(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<List<AppUser>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();  
        }
    }
}
