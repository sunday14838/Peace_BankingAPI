using MediatR;
using Peace_BankingAPI.Application.DTOs.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peace_BankingAPI.Application.Features.Transactions.Commands
{
    public class TransferCommand : IRequest<TransferResultDto>
    {
        public string FromAccountNumber { get; set; } = string.Empty;
        public string ToAccountNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "NGN";
        public string? Narration { get; set; }
    }
}
