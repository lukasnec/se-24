using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using se_24.backend.Entities;
using se_24.backend.src.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using BCrypt.Net;

namespace se_24.backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public UserController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config; // Injected IConfiguration to access appsettings.json
        }

        // POST: api/User/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto model)
        {
            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Username == model.Username))
                return BadRequest("User already exists");

            // Hash the password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // Create new user
            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = hashedPassword,
                Role = "User" // Default role
            };

            // Save user to database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully!");
        }

        // POST: api/User/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto model)
        {
            // Find the user
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials");

            // Generate JWT token
            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(User user)
        {
            var jwtKey = _config["JwtSettings:Key"]; // Retrieve key from appsettings.json
            var jwtIssuer = _config["JwtSettings:Issuer"];
            var jwtAudience = _config["JwtSettings:Audience"];

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = jwtIssuer,
                Audience = jwtAudience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    // DTOs for user input
    public class UserRegisterDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserLoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
