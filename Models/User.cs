using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SelfWallet.Models
{
    public class User
    {
        [Key]
        [Required(ErrorMessage = "Required field ! please set the value.")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "Required field ! please set the value.")]
        public Guid UserCode { get; set; }
        [Required(ErrorMessage = "Required field ! please set the value.")]
        [MaxLength(100,ErrorMessage ="User name cross the maximum length !")]
        public string UserName { get; set; }
        [EmailAddress(ErrorMessage ="Invalid email address !")]
        public string UserEmail { get; set; }
        [Required(ErrorMessage = "Required field ! please set the value.")]
        [RegularExpression("[a-z0-9]", ErrorMessage = "Password must have a-z and 0-9")]
        [MinLength(5, ErrorMessage = "Password must have 5 characters !")]
        public string UserPassword { get; set; }
        [Required(ErrorMessage = "Required field ! please set the value.")]
        [RegularExpression("[0-9]", ErrorMessage = "Mobile number must have 0-9 characters !")]
        [MinLength(11, ErrorMessage = "Password must have 11 characters !")]
        public string UserMobile { get; set; }
        public string UserImage { get; set; }
        [Required(ErrorMessage = "Required field ! please set the value.")]
        public DateTime AddedTime { get; set; }
        [Required(ErrorMessage = "Required field ! please set the value.")]
        public string IpAddress { get; set; }
        public IEnumerable<Account> Accounts { get; set; }
    }
}
