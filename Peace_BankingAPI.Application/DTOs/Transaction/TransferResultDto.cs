using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peace_BankingAPI.Application.DTOs.Transaction
{
    public class TransferResultDto
    {
        public bool Success { get; set; }
        public Guid TransactionId { get; set; }
        public string Reference { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string FromAccountNumber { get; set; } = string.Empty;
        public string ToAccountNumber { get; set; } = string.Empty;
        public decimal FromAccountNewBalance { get; set; }
        public decimal ToAccountNewBalance { get; set; }
        public string? Narration { get; set; }
        public DateTime CompletedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string? Message { get; set; }
    }
}
