using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using TradeApp.Data;
using TradeApp.Dtos;
using TradeApp.Entities;
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

    }
}
