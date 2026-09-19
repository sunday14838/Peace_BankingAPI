using MediatR;
using Peace_BankingAPI.Application.DTOs.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peace_BankingAPI.Application.Features.Accounts.Queries
{
    public class GetUserAccountsQuery : IRequest<IEnumerable<AccountDto>>
    {
        public Guid UserId { get; set; }
    }
}
