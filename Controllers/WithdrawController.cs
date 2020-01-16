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
    public class WithdrawController : ControllerBase
    {
        private readonly WalletContext _context;

        public WithdrawController(WalletContext context)
        {
            _context = context;
        }

        // GET: api/Withdraw
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WithdrawViewModel>>> GetTransactions()
        {
            return await _context.Transactions
                .Where(x=>x.Amount < 0)
                .Select(x => new WithdrawViewModel
                {
                    AccountCode = x.Account.AccountCode,
                    TransactionDate = x.AddedTime,
                    Amount = x.Amount * (-1),
                    TransactionCode = x.TransactionCode,
                    Account = new AccountViewModel
                    {
                        AccountName = x.Account.AccountName,
                        AccountPercent = x.Account.AccountPercent,
                        UserCode = x.Account.User.UserCode,
                        User = new UserViewModel
                        {
                            UserName = x.Account.User.UserName,
                            UserEmail = x.Account.User.UserEmail,
                            UserMobile = x.Account.User.UserMobile,

                        }
                    }
                })
                .ToListAsync();
        }

        // GET: api/Withdraw/5
        [HttpGet("{TransactionCode}")]
        public async Task<ActionResult<WithdrawViewModel>> GetTransaction(Guid TransactionCode)
        {
            var transaction = await _context.Transactions
                 .Select(x => new WithdrawViewModel
                 {
                     AccountCode = x.Account.AccountCode,
                     TransactionDate = x.AddedTime,
                     Amount = x.Amount*(-1),
                     TransactionCode = x.TransactionCode,
                     Account = new AccountViewModel
                     {
                         AccountName = x.Account.AccountName,
                         AccountPercent = x.Account.AccountPercent,
                         UserCode = x.Account.User.UserCode,
                         User = new UserViewModel
                         {
                             UserName = x.Account.User.UserName,
                             UserEmail = x.Account.User.UserEmail,
                             UserMobile = x.Account.User.UserMobile,

                         }
                     }
                 })
                .FirstOrDefaultAsync(x=>x.TransactionCode == TransactionCode);

            if (transaction == null)
            {
                return NotFound();
            }

            return transaction;
        }

        // PUT: api/Withdraw/5
       
        [HttpPut("{TransactionCode}")]
        public async Task<IActionResult> PutTransaction(Guid TransactionCode, WithdrawViewModel transaction)
        {
            if (TransactionCode != transaction.TransactionCode)
            {
                return BadRequest();
            }
            var existingData = await _context.Transactions.FirstOrDefaultAsync(x => x.TransactionCode == TransactionCode);
           if(existingData == null)
            {
                return NotFound();
            }
            var existingAccount = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountCode == transaction.AccountCode);
            if(existingAccount == null)
            {
                return BadRequest();
            }

            existingData.Amount = transaction.Amount * (-1);
            existingData.AddedTime =(DateTime) transaction.TransactionDate;
            existingData.AccountId = existingAccount.AccountId;
            _context.Entry(existingData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(TransactionCode))
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

        // POST: api/Withdraw
      
        [HttpPost]
        public async Task<ActionResult<Transaction>> PostTransaction(WithdrawViewModel transaction)
        {
            var existingAccount = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountCode == transaction.AccountCode);
            if (existingAccount == null)
            {
                return BadRequest();
            }
            Transaction model = new Transaction();
            model.TransactionCode = Guid.NewGuid();
            model.AccountId = existingAccount.AccountId;
            model.AddedTime = (DateTime)transaction.TransactionDate;
            model.Amount = transaction.Amount * (-1);
            model.CommonCode = Guid.NewGuid();
            model.IpAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            _context.Transactions.Add(model);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Withdraw/5
        [HttpDelete("{TransactionCode}")]
        public async Task<ActionResult<Transaction>> DeleteTransaction(Guid TransactionCode)
        {
            var transaction = await _context.Transactions.FirstOrDefaultAsync(x=>x.TransactionCode==TransactionCode);
            if (transaction == null)
            {
                return NotFound();
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return transaction;
        }

        private bool TransactionExists(Guid id)
        {
            return _context.Transactions.Any(e => e.TransactionCode == id);
        }
    }
}
