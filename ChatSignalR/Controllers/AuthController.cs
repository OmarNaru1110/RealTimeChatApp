using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Dtos;
using Server.Models;
using Server.Repositories.IRepositories;
using System.Text.Encodings.Web;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(UserDto user)
        {
            var result = await _authRepository.Register(user);
            if (result == false)
                return BadRequest("something went wrong");
            return Created();
        }

        [HttpGet("signIn")]
        public async Task<IActionResult> SignInAsync(UserDto user)
        {
            var result = await _authRepository.SignIn(user);
            if (result == null)
                return BadRequest("something went wrong");
            return Ok(result);
        }

    }
}
