using System;
using System.ComponentModel.DataAnnotations;
using BankAccounts.Models;

namespace BankAccounts.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }

        [Required]
        [Display(Name = "Deposit/Withdraw: ")]
        public int Amount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int UserID { get; set; }

        public User AccountHolder { get; set; }

    }
}
