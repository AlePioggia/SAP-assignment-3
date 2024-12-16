using Microsoft.AspNetCore.Mvc;
using UserService.application;

namespace UserService.controller
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ISessionService _redisSessionService;

        public UserController(IUserService userService, ISessionService redisSessionService)
        {
            _userService = userService;
            _redisSessionService = redisSessionService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto registrationDto)
        {
            if (registrationDto == null || string.IsNullOrWhiteSpace(registrationDto.UserName) || string.IsNullOrWhiteSpace(registrationDto.Password))
            {
                return BadRequest("Invalid registration details.");
            }

            var userId = await _userService.RegisterUser(registrationDto.UserName, registrationDto.Password);

            await _redisSessionService.StoreSessionAsync(userId, registrationDto.UserName);

            return CreatedAtAction(nameof(GetUserById), new { id = userId }, null);
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] UserAuthenticationDto authenticationDto)
        {
            if (authenticationDto == null || string.IsNullOrWhiteSpace(authenticationDto.UserName) || string.IsNullOrWhiteSpace(authenticationDto.Password))
            {
                return BadRequest("Invalid authentication details.");
            }

            var userId = await _userService.AuthenticateUser(authenticationDto.UserName, authenticationDto.Password);
            if (userId == null)
            {
                return Unauthorized();
            }

            await _redisSessionService.StoreSessionAsync(userId, authenticationDto.UserName);

            return Ok(new { UserId = userId });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet("{id}/credit")]
        public async Task<IActionResult> GetCredit(string id)
        {
            int userCredit = await _userService.GetCredit(id);
            return Ok(userCredit);
        }



        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("Server is up and running!");
        }
    }

    public class UserRegistrationDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class UserAuthenticationDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
