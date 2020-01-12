using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SelfWallet.Models.ViewModel
{
    public class AccountViewModel
    {
        [Required(ErrorMessage = "Required field ! please set the value.")]
        public Guid AccountCode { get; set; }
        [Required(ErrorMessage = "Required field ! please set the value.")]
        [MaxLength(100, ErrorMessage = "Account name cross the maximum length !")]
        [MinLength(3, ErrorMessage = "Account name must have 3 characters !")]
        public string AccountName { get; set; }
        [Required(ErrorMessage = "Required field ! please set the value.")]
        public float AccountPercent { get; set; }
        [Required(ErrorMessage = "Required field ! please set the value.")]
        public Guid UserCode { get; set; }
        public UserViewModel User { get; set; }
    }
}
