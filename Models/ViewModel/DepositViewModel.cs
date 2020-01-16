using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SelfWallet.Models.ViewModel
{
    public class DepositViewModel
    {
        public Guid? CommonCode { get; set; }
       
        [Required(ErrorMessage = "Required field ! please set the value.")]
        public float Amount { get; set; }
        public DateTime? TransactionDate { get; set; }
    }
}
