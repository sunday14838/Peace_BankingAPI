using MediatR;
using Peace_BankingAPI.Application.DTOs.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peace_BankingAPI.Application.Features.Accounts.Queries
{
    public class GetAccountQuery : IRequest<AccountDto>
    {
        public Guid Id { get; set; }
        public Guid RequestingUserId { get; set; }
    }
}
