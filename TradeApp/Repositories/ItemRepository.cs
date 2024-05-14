using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeApp.Data;
using TradeApp.Dtos;
using TradeApp.Entities;
using TradeApp.Interfaces;

namespace TradeApp.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public ItemRepository(DataContext context, IMapper mapper)
        {
            _context = context; 
            _mapper = mapper;
        }


        public async Task AddItemAsync(Item item)
        {
           await _context.Item.AddAsync(item);
        }

       

        public async Task<bool> DeleteItemAsync(int id)
        {
            var item = await _context.Item.FirstOrDefaultAsync(i => i.Id == id);

            if (item == null) return false;

            if(_context.Item.Remove(item) == null) return true;
            return false;
            
        }


        public Task<Item> GetItemByIdAsync(int id)
        {
            return _context.Item.Include(i => i.Photos).FirstOrDefaultAsync(i => i.Id == id);

        }


        //It must be finished
        public async Task<List<ItemPhoto>> GetItemPhotoByItemIdAsync(int itemId)
        {
            var item = await _context.Item.Include(i => i.Photos).FirstOrDefaultAsync(i => i.Id == itemId);

            var itemPhoto = item.Photos;

            
            return itemPhoto;
        }

        public Task<List<Item>> GetItemsAsync()
        {
            return _context.Item.Include(item => item.Owner).ToListAsync();
        }

        public Task<List<Item>> GetItemsByOwnerIdAsync(int ownerId)
        {
            return _context.Item.Where(i => i.OwnerId == ownerId).ToListAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
           return await _context.SaveChangesAsync() > 0;
        }

        public async Task UpdateItem(Item item)
        {
            _context.UpdateRange(item);
            await _context.SaveChangesAsync();
           //_context.Item.Update(item);
        }
    }
}
