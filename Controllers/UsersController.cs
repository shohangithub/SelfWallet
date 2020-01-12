using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SelfWallet.Models;
using SelfWallet.Models.ViewModel;

namespace SelfWallet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly WalletContext _context;

        public UsersController(WalletContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserViewModel>>> GetUsers()
        {
            return await _context.Users.Select(x => new UserViewModel
            {
                UserCode = x.UserCode,
                UserEmail = x.UserEmail,
                UserImage = x.UserImage,
                UserMobile = x.UserMobile,
                UserName = x.UserName,
                UserPassword = x.UserPassword
            }).ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{UserCode}")]
        public async Task<ActionResult<UserViewModel>> GetUser(Guid UserCode)
        {
            var user = await _context.Users
                .Select(x=> new UserViewModel
                {
                    UserCode = x.UserCode,
                    UserEmail = x.UserEmail,
                    UserImage = x.UserImage,
                    UserMobile = x.UserMobile,
                    UserName = x.UserName,
                    UserPassword = x.UserPassword
                })
                .FirstOrDefaultAsync(x => x.UserCode == UserCode);
            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        [HttpPut("{UserCode}")]
        public async Task<IActionResult> PutUser(Guid UserCode, UserViewModel model)
        {
           
            if (UserCode != model.UserCode)
            {
                return BadRequest();
            }

            var existingData =await _context.Users.FirstOrDefaultAsync(x => x.UserCode == UserCode);
                
                existingData.IpAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                existingData.AddedTime = DateTime.Now;
                existingData.UserEmail = model.UserEmail;
                existingData.UserImage = model.UserImage;
                existingData.UserMobile = model.UserMobile;
                existingData.UserName = model.UserName;
                existingData.UserPassword = model.UserPassword;
            

            _context.Entry(existingData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(UserCode))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(UserViewModel model)
        {
            User user = new User()
            {
                IpAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                AddedTime = DateTime.Now,
                UserCode = Guid.NewGuid(),
                UserEmail = model.UserEmail,
                UserImage = model.UserImage,
                UserMobile = model.UserMobile,
                UserName = model.UserName,
                UserPassword = model.UserPassword,
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.UserCode }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{UserCode}")]
        public async Task<ActionResult<User>> DeleteUser(Guid UserCode)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x=>x.UserCode==UserCode);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }
        private bool UserExists(Guid UserCode)
        {
            return _context.Users.Any(e => e.UserCode == UserCode);
        }
    }
}
