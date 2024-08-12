using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeApp.Dtos;
using TradeApp.Entities;
using TradeApp.Extensions;
using TradeApp.Helpers;
using TradeApp.Interfaces;

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
            var currentUser = await _userRepository.GetUserByUsernameAsync(userName);
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
           //var itemToReturn = _mapper.Map<ItemDto>(item);

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
        public async Task<ActionResult<PagedList<ItemDto>>> GetItems([FromQuery] UserParams userParams)
        {
            var items = await _itemRepository.GetItemsDtoAsync(userParams);

            Response.AddPaginationHeader(new PaginationHeader(items.CurrentPage, items.PageSize, items.TotalCount, items.TotalPages));
            //var itemToReturn = _mapper.Map<List<ItemDto>>(item);
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItemById(int id)
        {
            var item = await _itemRepository.GetItemByIdAsync(id);



            if (item != null) return Ok(_mapper.Map<ItemDto>(item));
            return NotFound("Item not found");
        }


        [HttpGet("personal")]
        public async Task<ActionResult<List<ItemDto>>> GetItemsByOwnerUsername()
        {
            var username = User.GetUsername();
            if (string.IsNullOrEmpty(username)) return Unauthorized("User not found");

            var items = await _itemRepository.GetItemsByOwnerUsernameAsync(username);

            if (items == null || items.Count == 0) return NotFound("Items not found");

            return Ok(items);
        }



        [Authorize]
        [HttpPost("{id}/addOffer")]
        public async Task<ActionResult> AddOffer(int id, AddOfferDto addOfferDto)
        {
            var userName = User.GetUsername();
            if (userName == null) return NotFound("Username in token not found");
            var currentUser = await _userRepository.GetUserByUsernameAsync(userName);
            if (currentUser == null) return Unauthorized();
            var item = await _itemRepository.GetItemByIdAsync(id);
            if (item == null) return NotFound();
            if (currentUser.Items != null && currentUser.Items.Contains(item)) return BadRequest("You can not offer yourself");
            var offerDto = new OfferDto
            {
                Comment = addOfferDto.OfferComment,
                PosterUsername = User.GetUsername(),
                Created = DateTime.UtcNow,
            };

            var offerToAdd = _mapper.Map<Offer>(offerDto);

            item.Offers.Add(offerToAdd);
            await _itemRepository.SaveChangesAsync();
            return Ok("Offer was successfully created");
        }

        [Authorize]
        [HttpGet("offers")]
        public async Task<ActionResult<OfferDto>> GetOffers()
        {
            //Must get only currentuser's offers + offer deleting function must be added
            var userName = User.GetUsername();
            if (userName == null) return NotFound("Username in token not found");
            var currentUser =  await _userRepository.GetUserByUsernameAsync(userName);
            if (currentUser == null) return Unauthorized();
            var items = await _itemRepository.GetItemsAsync();
            if (items == null) return NotFound();
            var offers = await _itemRepository.GetOfferDto(userName, items);
            if (offers == null) return NotFound();

            return Ok(offers);
        }

        [Authorize]
        [HttpDelete("offers/{id}")]
        public async Task<ActionResult> DeleteOffer(int id)
        {
            var username = User.GetUsername();

            var items = await _itemRepository.GetItemsAsync();
            if (items == null) return NotFound();
            var offer = items.SelectMany(i => i.Offers).Where(o => o.Id == id);
            if(offer == null) return NotFound();
            if (offer.FirstOrDefault().PosterUsername != username) return Forbid();
            var offerToDelete = offer.FirstOrDefault();
            /**  var offers = items.SelectMany(i => i.Offers).Where(o => o.PosterUsername == username);
              if (offers == null) return NotFound();
              var offerToDelete = offers.FirstOrDefault(o => o.Id == id);
              if (offerToDelete == null) return NotFound();
              */
            var item = items.Where(i => i.Offers.Contains(offerToDelete)).FirstOrDefault();
            if (item == null) return NotFound();
            item.Offers.Remove(offerToDelete);

            if (await _itemRepository.SaveChangesAsync()) return NoContent();

            return BadRequest("Offer could not be deleted");

        }


        [Authorize]
        [HttpPost("{id}/itemPhoto/add")]
        public async Task<ActionResult<ItemPhotoDto>> AddItemPhoto(int id, IFormFile file)
        {
            var currentUser = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            if (currentUser == null) return Unauthorized();


            //Find the item
            var item = await _itemRepository.GetItemByIdAsync(id);
            if (item == null) return NotFound("Item was not found");
            if (!currentUser.Items.Contains(item)) return BadRequest("you do not have access for this item");
            var result = await _itemPhotoService.AddItemPhotoAsync(file);
            if (result.Error != null) return BadRequest(result.Error.Message);
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


            if (await _itemRepository.SaveChangesAsync())
            {
                return Created("Photo", _mapper.Map<ItemPhotoDto>(itemPhoto));
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

        [Authorize]
        [HttpPut("{id}/itemPhoto/set-main-photo/{itemPhotoId}")]
        public async Task<ActionResult> SetMainItemPhoto(int id, int itemPhotoId)
        {
            var currentUser = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            if (currentUser == null) return Unauthorized();
            var item = await _itemRepository.GetItemByIdAsync(id);
            if (item == null) return NotFound();
            if (!currentUser.Items.Contains(item)) return BadRequest("you do not have access for this item");

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

        [Authorize]
        [HttpDelete("{id}/itemPhoto/delete/{itemPhotoId}")]
        public async Task<ActionResult> DeleteItemPhoto(int id, int itemPhotoId)
        {
            var currentUser = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            if (currentUser == null) return Unauthorized();

            var item = await _itemRepository.GetItemByIdAsync(id);
            if (item == null) return NotFound();
            if (!currentUser.Items.Contains(item)) return BadRequest("you do not have access for this item");
            var itemPhoto = item.Photos.FirstOrDefault(x => x.Id == itemPhotoId);

            if (itemPhoto == null) return NotFound("Photo was not found for this item");

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
