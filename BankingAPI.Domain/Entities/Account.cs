using Peace_BankingAPI.Domain.Common;
using Peace_BankingAPI.Domain.Enums;
using Peace_BankingAPI.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peace_BankingAPI.Domain.Entities
{
    public class Account : BaseEntity
    {
        // Basic Account Information
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public Guid UserId { get; set; }

        // Financial Information
        public decimal Balance { get; set; }  // Current account balance
        public Currency Currency { get; set; } // Currency type
        public AccountType Type { get; set; } // Savings, Checking, etc.
        public AccountStatus Status { get; set; } // Active, Frozen, etc.

        // Limits
        public decimal DailyWithdrawalLimit { get; set; } = 5000; // Default $5000
        public decimal MonthlyTransferLimit { get; set; } = 20000; // Default $20000

        // Freeze Information
        public DateTime? LockedUntil { get; set; }
        public string? LockReason { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();


        public void Deposit(decimal amount, string? reference = null)
        {
            // Validate the deposit
            if (amount <= 0)
                throw new DomainException("Deposit amount must be greater than zero");

            if (Status != AccountStatus.Active)
                throw new DomainException($"Cannot deposit to a {Status} account");

            // Add the money
            Balance += amount;

            // We'll add transaction logging here later
        }

        public void Withdraw(decimal amount, string? reference = null)
        {
            // Validate the withdrawal
            if (amount <= 0)
                throw new DomainException("Withdrawal amount must be greater than zero");

            if (Status != AccountStatus.Active)
                throw new DomainException($"Cannot withdraw from a {Status} account");

            if (Balance < amount)
                throw new InsufficientFundsException($"Insufficient balance. Available: {Balance:C}, Requested: {amount:C}");

            // Deduct the money
            Balance -= amount;
        }

        public void Freeze(string reason)
        {
            Status = AccountStatus.Frozen;
            LockReason = reason;
            LockedUntil = DateTime.UtcNow.AddDays(30); // Freeze for 30 days
        }

        public void Unfreeze()
        {
            Status = AccountStatus.Active;
            LockReason = null;
            LockedUntil = null;
        }

        public void Close()
        {
            if (Balance > 0)
                throw new DomainException("Cannot close account with positive balance");

            Status = AccountStatus.Closed;
        }
    }
}
