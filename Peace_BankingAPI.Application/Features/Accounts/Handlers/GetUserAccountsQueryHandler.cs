using AutoMapper;
using MediatR;
using Peace_BankingAPI.Application.DTOs.Account;
using Peace_BankingAPI.Application.Features.Accounts.Queries;
using Peace_BankingAPI.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peace_BankingAPI.Application.Features.Accounts.Handlers
{
    public class GetUserAccountsQueryHandler
    : IRequestHandler<GetUserAccountsQuery, IEnumerable<AccountDto>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;

        public GetUserAccountsQueryHandler(
            IAccountRepository accountRepository,
            IMapper mapper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AccountDto>> Handle(
            GetUserAccountsQuery request,
            CancellationToken cancellationToken)
        {
            var accounts = await _accountRepository.GetByUserIdAsync(request.UserId);

            return _mapper.Map<IEnumerable<AccountDto>>(accounts);
        }
    }
}
