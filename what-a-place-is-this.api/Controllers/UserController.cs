using Microsoft.AspNetCore.Mvc;
using what_a_place_is_this.api.Models;
using what_a_place_is_this.api.Services;

namespace what_a_place_is_this.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private readonly UserService _service;
        private readonly TokenService _tokenService;

        public UserController(UserService service, TokenService tokenService)
        {
            _service = service;
            _tokenService = tokenService;

        }

        [HttpPost("signin")]
        public async Task<IActionResult> Signin([FromForm] UserModel user)
        {
            string token = "";
            await _service.SigninAsync(user);
            return Ok(token);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] string userName, [FromForm] string pass)
        {

            UserModel _user = new();
            _user.UserName = userName;
            _user.Pass = pass;
            if (await _service.Login(_user) is null)
            {
                return NotFound();
            }
            _user = await _service.Login(_user);
            string token = await _tokenService.GenerateToken(_user);
            return Ok(token);
        }
    }
}
