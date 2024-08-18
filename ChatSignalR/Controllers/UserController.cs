using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Dtos;
using Server.Hubs;
using Server.Repositories.IRepositories;
using System.Formats.Asn1;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IGroupRepository _groupRepository;

        public UserController(IUserRepository userRepository, 
            IGroupRepository groupRepository)
        {
            _userRepository = userRepository;
            _groupRepository = groupRepository;
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers(string username)
        {
            var result = await _userRepository.Search(username);
            return Ok(result);
        }
        [HttpGet("chats")]
        public async Task<IActionResult> GetChats(int userId)
        {
            var result = await _userRepository.GetChats(userId); 
            return Ok(result);
        }
        [HttpGet("groups")]
        public async Task<IActionResult> GetGroups(int userId)
        {
            var result = await _groupRepository.GetUserGroups(userId);
            return Ok(result);
        }
        [HttpPost("group")]
        public async Task<IActionResult> AddGroup(GroupDto dto)
        {
            await _groupRepository.AddGroup(dto);
            return Ok();
        }
        [HttpPut("group")]
        public async Task<IActionResult> UpdateGroup(GroupDto dto)
        {
            await _groupRepository.UpdateGroup(dto);
            return Ok();
        }
    }
}
