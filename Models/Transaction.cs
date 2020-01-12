using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SelfWallet.Models
{
    public class Transaction
    {
        [Key]
        [Required(ErrorMessage = "Required field ! please set the value.")]
        public long TransactionId { get; set; }
        [Required(ErrorMessage = "Required field ! please set the value.")]
        public Guid TransactionCode { get; set; }
        [Required(ErrorMessage = "Required field ! please set the value.")]
        public int AccountId { get; set; }
        [Required(ErrorMessage = "Required field ! please set the value.")]
        public Guid CommonCode { get; set; }
        [Required(ErrorMessage = "Required field ! please set the value.")]
        public float Amount { get; set; }
        [Required(ErrorMessage = "Required field ! please set the value.")]
        public DateTime AddedTime { get; set; }
        [Required(ErrorMessage = "Required field ! please set the value.")]
        public string IpAddress { get; set; }
        public Account Account { get; set; }
    }
}
