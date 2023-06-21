using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol.Plugins;
using RestProject.Models;

namespace RestProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AuthContext _context;
        private readonly IDataProtector _dataProtector;

        public UsersController(AuthContext context, IDataProtectionProvider dataProtectionProvider)
        {
            _context = context;
            _dataProtector = dataProtectionProvider.CreateProtector("UsersControllerPurpose");
        }

        // GET: api/Users
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var users = await (from b in _context.Users
                               select new User()
                               {
                                   User_id = b.User_id,
                                   Login = _dataProtector.Unprotect(b.Login),
                                   Password = _dataProtector.Unprotect(b.Password),
                                   Role = b.Role
                               }).ToListAsync();
            return users;
        }

        // GET: api/Users/5
        [Authorize]
        [HttpGet("{login}")]
        public async Task<ActionResult<User>> GetUser(string login)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }

            var users = await (from b in _context.Users
                         select new User()
                         {
                             User_id = b.User_id,
                             Login = _dataProtector.Unprotect(b.Login),
                             Password = _dataProtector.Unprotect(b.Password),
                             Role = b.Role
                         }).ToListAsync();
            var user = users.FirstOrDefault(i => i.Login == login);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'AuthContext.Users'  is null.");
            }

            if (!user.Role.Equals("admin") && !user.Role.Equals("user"))
            {
                return Problem("Wrong role! You can be admin or user");
            }

            var users = await (from b in _context.Users
                         select new User()
                         {
                             User_id = b.User_id,
                             Login = _dataProtector.Unprotect(b.Login),
                             Password = _dataProtector.Unprotect(b.Password),
                             Role = b.Role
                         }).ToListAsync();
            var userFound = users.FirstOrDefault(i => i.Login == user.Login);

            if (userFound == null)
            {
                var newUser = new User()
                {
                    User_id = users[users.Count-1].User_id+1,
                    Login = _dataProtector.Protect(user.Login),
                    Password = _dataProtector.Protect(user.Password),
                    Role = user.Role
                };
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetUser", new { login = newUser.Login }, newUser);
            }
            else
            {
                return Conflict("User already exists");
            }
        }

        // DELETE: api/Users/5
        [Authorize(Roles = "admin")]
        [HttpDelete("{login}")]
        public async Task<IActionResult> DeleteUser(string login)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var users = await (from b in _context.Users
                         select new User()
                         {
                             User_id = b.User_id,
                             Login = _dataProtector.Unprotect(b.Login),
                             Password = _dataProtector.Unprotect(b.Password),
                             Role = b.Role
                         }).ToListAsync();
            var user = users.FirstOrDefault(i => i.Login == login);

            if (user == null)
            {
                return NotFound();
            }
            var userToDelete = await _context.Users.FindAsync(user.User_id);

            _context.Users.Remove(userToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
