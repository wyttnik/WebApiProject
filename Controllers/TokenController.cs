using Microsoft.AspNetCore.DataProtection;
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
        private readonly IDataProtector _dataProtector;

        public TokenController(AuthContext context, IConfiguration config,
            IDataProtectionProvider dataProtectionProvider)
        {
            _context = context;
            _configuration = config;
            _dataProtector = dataProtectionProvider.CreateProtector("UsersControllerPurpose");
        }

        [HttpPost]
        public async Task<IActionResult> Post(LogPass data)
        {
            string login = data.login;
            string password = data.password;
            var users = await (from b in _context.Users
                              select new User()
                              {
                                  Login = _dataProtector.Unprotect(b.Login),
                                  Password = _dataProtector.Unprotect(b.Password),
                                  Role = b.Role
                              }).ToListAsync();

            var user = users.FirstOrDefault(i => i.Login == login);

            if (user != null)
            {
                if (!user.Password.Equals(password)) return BadRequest("Invalid username or password");

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
                    expires: DateTime.UtcNow.AddMinutes(300),
                    signingCredentials: signIn);
                var handler = new JwtSecurityTokenHandler();
                var secToken = handler.WriteToken(token);
                //foreach(var item in handler.ReadJwtToken(secToken).Claims)
                //{
                //    Console.WriteLine(item);

                //}
                return Ok(secToken);
            }
            else
            {
                return BadRequest("Invalid username or password");
            }
        }
    }
}
