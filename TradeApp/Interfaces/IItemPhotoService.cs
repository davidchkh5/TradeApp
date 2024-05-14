using CloudinaryDotNet.Actions;

namespace TradeApp.Interfaces
{
    public interface IItemPhotoService
    {
        Task<ImageUploadResult> AddItemPhotoAsync(IFormFile file);
        Task<DeletionResult> DeleteItemPhotoAsync(string photoId);
    }
}
