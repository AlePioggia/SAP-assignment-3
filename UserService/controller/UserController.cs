using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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

            var token = GenerateJwtToken(authenticationDto);

            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(UserAuthenticationDto user) {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SecretKey"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "Issuer",
                audience: "Audience",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
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
