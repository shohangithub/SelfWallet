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
    public class TransactionsController : ControllerBase
    {
        private readonly WalletContext _context;

        public TransactionsController(WalletContext context)
        {
            _context = context;
        }

        // GET: api/Transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionViewModel>>> GetTransactions()
        {
            return await _context.Transactions
                .Select(x => new TransactionViewModel
                {
                    AccountCode = x.Account.AccountCode,
                    AddedTime = x.AddedTime,
                    Amount = x.Amount,
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

        // GET: api/Transactions/5
        [HttpGet("{CommonCode}")]
        public async Task<ActionResult<TransactionViewModel>> GetTransaction(Guid CommonCode)
        {
            var transaction = await _context.Transactions
                .Select(x => new TransactionViewModel
                {
                    AccountCode = x.Account.AccountCode,
                    AddedTime = x.AddedTime,
                    Amount = x.Amount,
                    TransactionCode = x.TransactionCode,
                     CommonCode=x.CommonCode,
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
                .FirstOrDefaultAsync(x=>x.CommonCode == CommonCode);

            if (transaction == null)
            {
                return NotFound();
            }

            return transaction;
        }

        // PUT: api/Transactions/5
        [HttpPut("{CommonCode}")]
        public async Task<IActionResult> PutTransaction(Guid CommonCode, TransactionViewModel model)
        {
            if (CommonCode != model.CommonCode)
            {
                return BadRequest();
            }
            var existingTransaction = await _context.Transactions.Where(x => x.CommonCode == CommonCode).ToListAsync();
            if (existingTransaction.Count > 0)
            {
                var existingAccount = await _context.Accounts.ToListAsync();
                var updateableTransaction = existingTransaction.Where(x=>x.AccountId == x.AccountId).ToList();
                updateableTransaction.ForEach(x => x.Amount = x.Account.AccountPercent * x.Amount / 100);
                await _context.SaveChangesAsync();


                foreach (var item in existingTransaction)
                {
                    if (existingAccount.Any(x => x.AccountId == item.AccountId))
                    {
                        _context.Entry(item).State = EntityState.Modified;
                    }
                }
            }
            else
            {
                return NotFound();
            }

          

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(CommonCode))
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

        // POST: api/Transactions
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Transaction>> PostTransaction(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTransaction", new { id = transaction.TransactionId }, transaction);
        }

        // DELETE: api/Transactions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Transaction>> DeleteTransaction(long id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return transaction;
        }

        private bool TransactionExists(Guid CommonCode)
        {
            return _context.Transactions.Any(e => e.CommonCode == CommonCode);
        }
    }
}
