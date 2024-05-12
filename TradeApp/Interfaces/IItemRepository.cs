﻿using TradeApp.Dtos;
using TradeApp.Entities;

namespace TradeApp.Interfaces
{
    public interface IItemRepository
    {
        Task AddItemAsync(Item item);
        Task<List<Item>> GetItemsAsync();
        Task<Item> GetItemByIdAsync(int id);
        Task<List<Item>> GetItemsByOwnerIdAsync(int ownerId);
        Task<List<ItemPhoto>> GetItemPhotoByItemIdAsync(int itemId);
        Task<bool> DeleteItemAsync(int id);
        Task<bool> SaveChangesAsync();
        Task UpdateItem(Item item);

    }
}
