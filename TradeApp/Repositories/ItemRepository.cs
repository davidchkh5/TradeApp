﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TradeApp.Data;
using TradeApp.Dtos;
using TradeApp.Entities;
using TradeApp.Helpers;
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
            return _context.Item.Include(i => i.Photos).Include(i => i.Owner).Include(i => i.Offers).FirstOrDefaultAsync(i => i.Id == id);

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
            return _context.Item.Include(item => item.Owner).Include(i=> i.Offers).ToListAsync();
        }

        public Task<List<Item>> GetItemsByOwnerIdAsync(int ownerId)
        {
            return _context.Item.Where(i => i.OwnerId == ownerId).ToListAsync();
        }

        public async Task<PagedList<ItemDto>> GetItemsDtoAsync(UserParams userParams)
        {
            var query =  _context.Item.Include(i => i.Owner).ProjectTo<ItemDto>(_mapper.ConfigurationProvider).AsQueryable().AsNoTracking();
   
            return await PagedList<ItemDto>.CreateAsync(query, userParams.PageNumber, userParams.PageSize); ;
        }

        public async Task<List<OfferDto>> GetOfferDto(string username,List<Item> items)
        {
            var itemIds = items.Select(i => i.Id);

            var offers = await _context.Item.Where(i => itemIds.Contains(i.Id) && i.Owner.UserName == username).SelectMany(i => i.Offers).ToListAsync();

           

            var offersDto = _mapper.Map<List<OfferDto>>(offers);

            return offersDto;
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
