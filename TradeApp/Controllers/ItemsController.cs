using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeApp.Dtos;
using TradeApp.Entities;
using TradeApp.Interfaces;
using TradeApp.Extensions;
using AutoMapper;

namespace TradeApp.Controllers
{

    public class ItemsController : BaseApiController
    {
        private readonly IItemRepository _itemRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IItemPhotoService _itemPhotoService;
        public ItemsController(IItemRepository itemRepository, IUserRepository userRepository, IMapper mapper, IItemPhotoService itemPhotoService)
        {
            _itemRepository = itemRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _itemPhotoService = itemPhotoService;
        }


        [Authorize]
        [HttpPost("add")]
        public async Task<ActionResult> AddItem(AddItemDto addItemDto)
        {
            var userName = User.GetUsername();
            if (userName == null) return Unauthorized();
            var currentUser = await _userRepository.GetUserByUsername(userName);
            if (currentUser == null) return NotFound();

            var item = new Item
            {
                TradeFor = addItemDto.TradeFor,
                Description = addItemDto.Description,
                Name = addItemDto.Name,
                Owner = currentUser,
                MainPhotoUrl = addItemDto.ItemPhotoUrl,
                OwnerId = currentUser.Id, 
            };
            var itemToReturn = _mapper.Map<ItemDto>(item);

           await _itemRepository.AddItemAsync(item);
            if (await _itemRepository.SaveChangesAsync())
            {
                return Ok("Item Has Created Successfully");
            }
            else
            {
                return BadRequest("Could not be created an item");
            }                     
        }



        [HttpGet]
        public async Task<ActionResult<List<ItemDto>>> GetItems()
        {
            var item = await _itemRepository.GetItemsAsync();
            var itemToReturn = _mapper.Map<List<ItemDto>>(item);
            return itemToReturn ;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItemById(int id)
        {
           var item =  await _itemRepository.GetItemByIdAsync(id);

            

            if(item != null) return Ok(_mapper.Map<ItemDto>(item));
            return NotFound("Item not found");
        }

        [HttpGet("personal/{ownerId}")]

        public async Task<ActionResult<List<ItemDto>>> GetItemsByOwnerId(int ownerId)
        {
            var items = await _itemRepository.GetItemsByOwnerIdAsync(ownerId);
            
            if (items == null) return NotFound("this user does not have items");
            return Ok(_mapper.Map<ItemDto>(items));
        }


        //Not Tested!

        [HttpPost("{id}/itemPhoto/add")]
        public async Task<ActionResult<ItemPhotoDto>> AddItemPhoto(int id,IFormFile file) 
        {
            //Find the item
            var item = await _itemRepository.GetItemByIdAsync(id);
            if (item == null) return NotFound("Item was not found");
            var result = await _itemPhotoService.AddItemPhotoAsync(file);
            if(result.Error != null) return BadRequest(result.Error.Message);
            var itemPhoto = new ItemPhoto()
            {
                PhotoUrl = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (item.Photos.Count == 0) itemPhoto.IsMain = true;

            item.Photos.Add(itemPhoto);

            //Add in List the object
          /**    var itemPhotoToAdd = new ItemPhoto()
                 {
                      PhotoUrl = addPhotoItemDto.PhotoUrl,
               Item = item,
                ItemId = item.Id
              }; 
            item.Photos.Add(photo); 
          
            */


            if(await _itemRepository.SaveChangesAsync())
            {
                return Created("Photo",_mapper.Map<ItemPhotoDto>(itemPhoto));
            }

            return BadRequest("Problem adding item photo");

        /* await _itemRepository.UpdateItem(item);

            var itemToReturn = _mapper.Map<List<ItemPhotoDto>>(item.Photos);

            await _itemRepository.SaveChangesAsync();

            return Ok(itemToReturn);   
        */
        } 
        

        [HttpGet("{id}/itemPhoto")]
        public async Task<ActionResult<List<ItemPhotoDto>>> GetItemPhotoByItemId(int id)
        {
            var photos = await _itemRepository.GetItemPhotoByItemIdAsync(id);
            var photosToReturn = _mapper.Map<List<ItemPhotoDto>>(photos);
            return photosToReturn;
        }

        [HttpPut("{id}/itemPhoto/set-main-photo/{itemPhotoId}")]
        public async Task<ActionResult> SetMainItemPhoto(int id,int itemPhotoId)
        {
            var item = await _itemRepository.GetItemByIdAsync(id);

            if (item == null) return NotFound();

            var photo = item.Photos.FirstOrDefault(x => x.Id == itemPhotoId);
            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("This is already item's main photo");

            var currentMain = item.Photos.FirstOrDefault(x => x.IsMain);

            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            if (await _itemRepository.SaveChangesAsync()) return NoContent();

            return BadRequest("Problem setting the main photo");
        }


        /**   [HttpPut("{id}/itemPhoto/delete/{itemPhotoId}")]
           public async Task<ActionResult> DeleteItemPhoto(int id, int itemPhotoId)
           {
               var item = await _itemRepository.GetItemByIdAsync(id);

               if (item.Photos.Count == 0) return NotFound("no photos found");

               var photoToDelete = item.Photos.FirstOrDefault(p => p.Id == itemPhotoId);
               if (photoToDelete == null) return NotFound("Photo for this Id not found");
               if (!item.Photos.Contains(photoToDelete)) return NotFound("Photo to delete could not be found");
               if (item.Photos.Remove(photoToDelete))
               {
                   await _itemRepository.UpdateItem(item);
                   await _itemRepository.SaveChangesAsync();
                   return Ok();

               }else
               {
                   return BadRequest("Photo could not be deleted");
               }
        
           }*/

        [HttpDelete("{id}/itemPhoto/delete/{itemPhotoId}")]
        public async Task<ActionResult> DeleteItemPhoto(int id,int itemPhotoId)
        {
            var item = await _itemRepository.GetItemByIdAsync(id);

            var itemPhoto = item.Photos.FirstOrDefault(x => x.Id == itemPhotoId);

            if (itemPhoto == null) return NotFound("Photo was not found");

            if (itemPhoto.IsMain) return BadRequest("You can not delete your main photo");

            if (itemPhoto.PublicId != null)
            {
                var result = await _itemPhotoService.DeleteItemPhotoAsync(itemPhoto.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            item.Photos.Remove(itemPhoto);

            if (await _itemRepository.SaveChangesAsync()) return Ok();

            return BadRequest("Problem deleting photo");
        }
    }
}
