using Peace_BankingAPI.Domain.Common;
using Peace_BankingAPI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peace_BankingAPI.Domain.Entities
{
    public class Transaction : BaseEntity
    {
        public Guid SourceAccountId { get; set; }
        public Guid? DestinationAccountId { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public TransactionType Type { get; set; }
        public TransactionStatus Status { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string? Narration { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? FailureReason { get; set; }

        // Navigation properties
        public virtual Account SourceAccount { get; set; } = null!;
        public virtual Account? DestinationAccount { get; set; }
    }
}
