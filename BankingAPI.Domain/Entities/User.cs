using Peace_BankingAPI.Domain.Common;
using Peace_BankingAPI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peace_BankingAPI.Domain.Entities
{
    public class User : BaseEntity
    {
        // Identity
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        // Name
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        // Security
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Customer;

        // Account state
        public bool IsActive { get; set; } = true;
        public bool EmailConfirmed { get; set; } = false;

        // Audit
        public DateTime? LastLoginAt { get; set; }

        // Navigation (a user has many accounts)
        public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

        // Convenience property (not persisted; computed at runtime)
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
