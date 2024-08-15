using TradeApp.Dtos;
using TradeApp.Entities;
using TradeApp.Helpers;

namespace TradeApp.Interfaces
{
    public interface IItemRepository
    {
        Task AddItemAsync(Item item);
        Task<List<Item>> GetItemsAsync();
        Task<PagedList<ItemDto>> GetItemsDtoAsync(UserParams userParams);
        Task<Item> GetItemByIdAsync(int id);
        Task<List<OfferDto>> GetOfferDto(string username,List<Item> items);
        Task<List<Item>> GetItemsByOwnerIdAsync(int ownerId);
        Task<List<ItemDto>> GetItemsByOwnerUsernameAsync(string username);
        Task<List<ItemPhoto>> GetItemPhotoByItemIdAsync(int itemId);
        Task<bool> DeleteItemAsync(Item item);
        Task<bool> SaveChangesAsync();
        Task UpdateItem(Item item);

    }
}
