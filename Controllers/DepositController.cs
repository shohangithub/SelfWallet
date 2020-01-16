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
    public class DepositController : ControllerBase
    {
        private readonly WalletContext _context;

        public DepositController(WalletContext context)
        {
            _context = context;
        }

        // GET: api/Deposit
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepositViewModel>>> GetDeposits()
        {
            return await _context.Transactions
            .Where(x=>x.Amount>0)
                .GroupBy(x=>new {
            x.AddedTime,
            x.CommonCode
            }).Select(x => new DepositViewModel {
                Amount = x.Sum(y=>y.Amount),
                CommonCode=x.Key.CommonCode,
                TransactionDate=x.Key.AddedTime
            }).ToListAsync();
        }

        // GET: api/Deposit/5
        [HttpGet("{CommonCode}")]
        public async Task<ActionResult<DepositViewModel>> GetDeposit(Guid CommonCode)
        {
            var transaction = await _context.Transactions.GroupBy(x => new {
                x.AddedTime,
                x.CommonCode
            }).Select(x => new DepositViewModel
            {
                Amount = x.Sum(y => y.Amount),
                CommonCode = x.Key.CommonCode,
                TransactionDate = x.Key.AddedTime
            }).FirstOrDefaultAsync(x => x.CommonCode == CommonCode);

            if (transaction == null)
            {
                return NotFound();
            }

            return transaction;
        }

        // PUT: api/Deposit/5
        [HttpPut("{CommonCode}")]
        public async Task<IActionResult> PutTransaction(Guid CommonCode, DepositViewModel transaction)
        {
            if (CommonCode != transaction.CommonCode)
            {
                return BadRequest();
            }
            var existingTransaction = await _context.Transactions.Where(x => x.CommonCode == CommonCode).ToListAsync();
            if (existingTransaction.Count > 0)
            {
                var existingAccount = await _context.Accounts.ToListAsync();
                var updateableTransaction = existingTransaction.Where(x => x.AccountId == x.AccountId).ToList();
                updateableTransaction.ForEach(x => x.Amount = x.Account.AccountPercent * x.Amount / 100);
                await _context.SaveChangesAsync();


                foreach (var item in existingTransaction)
                {
                    if (existingAccount.Any(x => x.AccountId == item.AccountId))
                    {
                        _context.Entry(item).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
            }
            else
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/Deposit
        [HttpPost]
        public async Task<ActionResult> PostDeposit(DepositViewModel transaction)
        {
            List<Transaction> transactions = new List<Transaction>();
            var CommonCode = Guid.NewGuid();
            var dateTime = transaction.TransactionDate;

            var existingAccount = await _context.Accounts.Select(x => new Transaction {
                AccountId = x.AccountId,
                AddedTime = (DateTime)dateTime,
                Amount = x.AccountPercent * transaction.Amount / 100,
                CommonCode = CommonCode,
                IpAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                TransactionCode = Guid.NewGuid()
            }).ToListAsync();

            _context.Transactions.AddRange(existingAccount);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Deposit/5
        [HttpDelete("{CommonCode}")]
        public async Task<ActionResult> DeleteTransaction(Guid CommonCode)
        {
            var transaction = await _context.Transactions.Where(x=>x.CommonCode == CommonCode).ToListAsync();
            if (transaction == null)
            {
                return NotFound();
            }

            try
            {
                _context.Transactions.RemoveRange(transaction);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
