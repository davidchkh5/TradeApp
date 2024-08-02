using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using TradeApp.Data;
using TradeApp.Dtos;
using TradeApp.Entities;
using TradeApp.Extensions;
using TradeApp.Interfaces;

namespace TradeApp.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<MemberDto>>> GetUsers()
        {
            var users = await _userRepository.GetUsersAsync();

           var usersToReturn = _mapper.Map<List<MemberDto>>(users);

            return Ok(usersToReturn);
        }


        [Authorize]
        [HttpGet("{userName}")]
        public async Task<ActionResult<MemberDto>> GetUser(string userName)
        {
          
            var user = await _userRepository.GetUserByUsernameAsync(userName);
            if (user == null) return NotFound("User was not found");

            var userToReturn = _mapper.Map<MemberDto>(user);
            return Ok(userToReturn);
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<ActionResult<MemberDto>> UpdateUser(UpdateUserDto updateUserDto)
        {
            var currentUser = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            if (currentUser == null) return Unauthorized();
            //Validation of email and phone number before updating
            var users = await _userRepository.GetUsersAsync();
            var usersEmails = users.Select(u => u.Email).AsQueryable();
            var usersPhoneNumber = users.Select(u => u.PhoneNumber).AsQueryable();
            if (!currentUser.Email.Equals(updateUserDto.Email))
            {
                if (usersEmails.Contains(updateUserDto.Email)) return BadRequest("This email address is already taken"); 
            }

            if(!currentUser.PhoneNumber.Equals(updateUserDto.PhoneNumber))
            {
                if (usersPhoneNumber.Contains(updateUserDto.PhoneNumber)) return BadRequest("This phone number is already taken");
            }

           _mapper.Map(updateUserDto, currentUser);
        /*    var userToReturn = _mapper.Map<MemberDto>(userToUpdate);
            await _userRepository.UpdateUserAsync(userToUpdate);
            */
            if (!await _userRepository.SaveChangesAsync()) return BadRequest("Failed to update the user");

            return NoContent();
        }



    }
}
