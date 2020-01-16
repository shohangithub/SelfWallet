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
    public class AccountsController : ControllerBase
    {
        private readonly WalletContext _context;

        public AccountsController(WalletContext context)
        {
            _context = context;
        }

        // GET: api/Accounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountViewModel>>> GetAccounts()
        {
            return await _context.Accounts.Select(x =>
            new AccountViewModel
            {
                AccountCode = x.AccountCode,
                AccountName = x.AccountName,
                AccountPercent = x.AccountPercent,
                UserCode = x.User.UserCode,
                User = new UserViewModel
                {
                    UserEmail = x.User.UserEmail,
                    UserImage = x.User.UserImage,
                    UserMobile = x.User.UserMobile,
                    UserName = x.User.UserName,
                }
            }).ToListAsync();
        }

        // GET: api/Accounts/5
        [HttpGet("{AccountCode}")]
        public async Task<ActionResult<AccountViewModel>> GetAccount(Guid AccountCode)
        {
            var account = await _context.Accounts.Select(x =>
            new AccountViewModel
            {
                AccountCode = x.AccountCode,
                AccountName = x.AccountName,
                AccountPercent = x.AccountPercent,
                UserCode = x.User.UserCode,
                User = new UserViewModel
                {
                    UserEmail = x.User.UserEmail,
                    UserImage = x.User.UserImage,
                    UserMobile = x.User.UserMobile,
                    UserName = x.User.UserName,
                }
            }).FirstOrDefaultAsync(x => x.AccountCode == AccountCode);

            if (account == null)
            {
                return NotFound();
            }

            return account;
        }

        // PUT: api/Accounts/5
        [HttpPut("{AccountCode}")]
        public async Task<IActionResult> PutAccount(Guid AccountCode, AccountViewModel model)
        {
            if (AccountCode != model.AccountCode)
            {
                return BadRequest();
            }
            var account = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountCode == model.AccountCode);
            if (account == null)
            {
                return BadRequest();
            }
            account.AccountCode = model.AccountCode;
            account.AccountName = model.AccountName;
            account.AccountPercent = model.AccountPercent;
            account.AddedTime = DateTime.Now;
            account.IpAddress = HttpContext.Connection.RemoteIpAddress.ToString();
 
            _context.Entry(account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(AccountCode))
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

        // POST: api/Accounts
        [HttpPost]
        public async Task<ActionResult<Account>> PostAccount(AccountViewModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserCode == model.UserCode);
            if (user == null) return BadRequest();

            var account = new Account()
            {
                AccountCode = Guid.NewGuid(),
                AccountName = model.AccountName,
                AccountPercent = model.AccountPercent,
                UserId = user.UserId,
                IpAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                AddedTime = DateTime.Now
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return RedirectToAction("GetAccount", new { AccountCode = account.AccountCode });
        }

        // DELETE: api/Accounts/5
        [HttpDelete("{AccountCode}")]
        public async Task<ActionResult<Account>> DeleteAccount(Guid AccountCode)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountCode == AccountCode);
            if (account == null)
            {
                return NotFound();
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return account;
        }

        private bool AccountExists(Guid AccountCode)
        {
            return _context.Accounts.Any(e => e.AccountCode == AccountCode);
        }
    }
}
