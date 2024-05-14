using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using TradeApp.Helpers;
using TradeApp.Interfaces;

namespace TradeApp.Services
{
    public class ItemPhotoService : IItemPhotoService
    {

        private readonly Cloudinary _cloudinary;
        public ItemPhotoService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account
            {
                Cloud = config.Value.CloudName,
                ApiKey = config.Value.ApiKey,
                ApiSecret = config.Value.ApiSecret,

            };

            _cloudinary = new Cloudinary(acc);
            
        }

            public async Task<ImageUploadResult> AddItemPhotoAsync(IFormFile file)
            {

                var uploadResult = new ImageUploadResult();

               if(file.Length > 0)
                {

                    using var stream = file.OpenReadStream();
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                        Folder = "ta-net7"
                    }; 
    

                    uploadResult = await _cloudinary.UploadAsync(uploadParams);
                }
                return uploadResult;
            }

        public async Task<DeletionResult> DeleteItemPhotoAsync(string photoId)
        {
            var deleteParams = new DeletionParams(photoId);

            return await _cloudinary.DestroyAsync(deleteParams);
        }
    }

}
