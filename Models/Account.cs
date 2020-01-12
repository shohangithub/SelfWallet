using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SelfWallet.Models
{
    public class Account
    {
        [Key]
        [Required(ErrorMessage = "Required field ! please set the value.")]
        public int AccountId { get; set; }
        [Required(ErrorMessage ="Required field ! please set the value.")]
        public Guid AccountCode { get; set; }
        [Required(ErrorMessage = "Required field ! please set the value.")]
        [MaxLength(100, ErrorMessage = "Account name cross the maximum length !")]
        [MinLength(3, ErrorMessage = "Account name must have 3 characters !")]
        public string AccountName { get; set; }
        [Required(ErrorMessage = "Required field ! please set the value.")]
        public float AccountPercent { get; set; }
        [Required(ErrorMessage = "Required field ! please set the value.")]
        public DateTime AddedTime { get; set; }
        [Required(ErrorMessage = "Required field ! please set the value.")]
        public string IpAddress { get; set; }
        [Required(ErrorMessage = "Required field ! please set the value.")]
        public int UserId { get; set; }

        public User User { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
    }
}
