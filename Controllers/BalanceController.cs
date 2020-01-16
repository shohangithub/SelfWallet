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
    public class BalanceController : ControllerBase
    {
        private readonly WalletContext _context;
        public BalanceController(WalletContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Balance>>> GetBalance()
        {
            return await _context.Transactions.GroupBy(x => new
            {
                x.Account.AccountName
            }).Select(x => new Balance
            {
                AccountName = x.Key.AccountName,
                Amount = x.Sum(x => x.Amount)
            }).ToListAsync();
        }
    }
}