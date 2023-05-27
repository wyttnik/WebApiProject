using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestProject.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RestProject.Controllers
{
    [Route("api/token")]
    [ApiController]
    public class TokenController: ControllerBase
    {
        private IConfiguration _configuration;
        private readonly AuthContext _context;

        public TokenController(AuthContext context, IConfiguration config)
        {
            _context = context;
            _configuration = config;
        }

        [HttpPost]
        public async Task<IActionResult> Post(string login, string password)
        {
                var user = await _context.Users.FindAsync(login);

                if (user != null)
                {
                    if (!user.Password.Equals(password)) return BadRequest("Invalid password");

                //create claims details based on the user information
                    var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(5),
                        signingCredentials: signIn);

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest("Invalid credentials");
                }
        }
    }
}
