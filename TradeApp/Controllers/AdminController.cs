using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeApp.Entities;
using TradeApp.Interfaces;

namespace TradeApp.Controllers
{
    public class AdminController :BaseApiController
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly IItemRepository _itemRepository;
        public AdminController(UserManager<AppUser> userManager, IItemRepository itemRepository)
        {
            _userManager = userManager;
            _itemRepository = itemRepository;
        }

        [Authorize(Policy = "RequiredAdminRole")]
        [HttpGet("users-with-roles")]

        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await _userManager.Users.OrderBy(u => u.UserName).Select(u => new
            {
                u.Id,
                Username = u.UserName,
                Roles = u.UserRoles.Select(r => r.Role).ToList(),
            }).ToListAsync();

            return Ok(users);
        }


        [Authorize(Policy = "RequiredAdminRole")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditUserRoles(string username, [FromQuery] string roles)
        {
            if (roles == null) return BadRequest("Select at least 1 role");
            var selectedRoles = roles.Split(",").ToArray();
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return NotFound("user not found");

            var userCurrentRoles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userCurrentRoles));

            if (!result.Succeeded) return BadRequest("Roles can not be added");

            result = await _userManager.RemoveFromRolesAsync(user, userCurrentRoles.Except(userCurrentRoles));

            if (!result.Succeeded) return BadRequest("Previous roles can not be removed");

            return Ok(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = "RequiredAdminRole")]
        [HttpDelete("delete-user/{username}")]
        public async Task<ActionResult> DeleteUser(string username)
        {
           var user =  await _userManager.FindByNameAsync(username);
            if (user == null) return NotFound("User was not found");

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin") || roles.Contains("Moderator")) return BadRequest("you can not deactive admin's or moderator's user");
            var result = _userManager.DeleteAsync(user);
            if (!result.IsCompletedSuccessfully) return BadRequest("the user can not be deleted");

            return Ok("user was deleted successfully");
        }

        [Authorize(Policy = "RequiredAdminRole")]
        [HttpDelete("delete-item/{itemId}")]
        public async Task<ActionResult> DeleteItem(int ItemId)
        {
           var item = await _itemRepository.GetItemByIdAsync(ItemId);
            if (item == null) return NotFound("Item was not found");
            if (await _itemRepository.DeleteItemAsync(ItemId) == false) return BadRequest("The item could not be deleted");
            return Ok("The itme has deleted successfully");
        }

    }
}
