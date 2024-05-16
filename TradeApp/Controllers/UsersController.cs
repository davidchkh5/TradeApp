﻿using AutoMapper;
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
        [HttpPut("update")]
        public async Task<ActionResult<MemberDto>> UpdateUser(UpdateUserDto updateUserDto)
        {
            var currentUser = await _userRepository.GetUserByUsername(User.GetUsername());
            if (currentUser == null) return Unauthorized();
            //Validation of email and phone number before updating
            var users = await _userRepository.GetUsersAsync();
            var usersEmails = users.Select(u => u.Email).AsQueryable();
            var usersPhoneNumber = users.Select(u => u.PhoneNumber).AsQueryable();
            if (usersPhoneNumber.Contains(updateUserDto.PhoneNumber)) return BadRequest("This phone number is already taken");
            if (usersEmails.Contains(updateUserDto.Email)) return BadRequest("This email address is already taken");
           _mapper.Map(updateUserDto, currentUser);
        /*    var userToReturn = _mapper.Map<MemberDto>(userToUpdate);
            await _userRepository.UpdateUserAsync(userToUpdate);
            */
            if (!await _userRepository.SaveChangesAsync()) return BadRequest("Failed to update the user");

            return NoContent();
        }



    }
}
