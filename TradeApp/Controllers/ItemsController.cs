using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TradeApp.Dtos;
using TradeApp.Entities;
using TradeApp.Interfaces;
using TradeApp.Extensions;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TradeApp.Controllers
{
 
    public class ItemsController : BaseApiController
    {
        private readonly IItemRepository _itemRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public ItemsController(IItemRepository itemRepository, IUserRepository userRepository, IMapper mapper)
        {
            _itemRepository = itemRepository;
            _userRepository = userRepository;
            _mapper = mapper;
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

        [HttpPut("{id}/itemPhoto/add")]
        public async Task<ActionResult> AddItemPhoto(int id, AddItemPhotoDto addPhotoItemDto) 
        {
            //Find the item
            var item = await _itemRepository.GetItemByIdAsync(id);
            if (item == null) return NotFound("Item was not found"); 

            if(item.Photos.Count == 0)
            {
                //Create new object 
                item.Photos = new List<ItemPhoto>()
                {
                    new ItemPhoto()
                    {
                        PhotoUrl = addPhotoItemDto.PhotoUrl,
                        Item = item,
                        ItemId = item.Id
                    }
                };
            }   
            else
            {
                //Add in List the object
                var itemPhotoToAdd = new ItemPhoto()
                {
                    PhotoUrl = addPhotoItemDto.PhotoUrl,
                    Item = item,
                    ItemId = item.Id
                };

                item.Photos.Add(itemPhotoToAdd);
            }

           await _itemRepository.UpdateItem(item);

            var itemToReturn = _mapper.Map<List<ItemPhotoDto>>(item.Photos);

            await _itemRepository.SaveChangesAsync();

            return Ok(itemToReturn);   
        }

        [HttpGet("{id}/itemPhoto")]
        public async Task<ActionResult<List<ItemPhotoDto>>> GetItemPhotoByItemId(int id)
        {
            var photos = await _itemRepository.GetItemPhotoByItemIdAsync(id);
            var photosToReturn = _mapper.Map<List<ItemPhotoDto>>(photos);
            return photosToReturn;
        }

        
        
    }
}
