using MediatR;
using Peace_BankingAPI.Application.DTOs.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peace_BankingAPI.Application.Features.Accounts.Commands
{
    public class CreateAccountCommand : IRequest<AccountDto>
    {
        public Guid UserId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public string Currency { get; set; } = "NGN";
        public string AccountType { get; set; } = "Savings";
    }
}
