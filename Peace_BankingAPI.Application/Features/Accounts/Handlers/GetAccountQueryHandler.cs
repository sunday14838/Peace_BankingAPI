using AutoMapper;
using MediatR;
using Peace_BankingAPI.Application.DTOs.Account;
using Peace_BankingAPI.Application.Features.Accounts.Queries;
using Peace_BankingAPI.Domain.Exceptions;
using Peace_BankingAPI.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peace_BankingAPI.Application.Features.Accounts.Handlers
{
    public class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, AccountDto>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;

        public GetAccountQueryHandler(IAccountRepository accountRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        public async Task<AccountDto> Handle(GetAccountQuery request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(request.Id);

            if (account == null)
                throw new AccountNotFoundException($"Account with ID {request.Id} not found");

            if (account.UserId != request.RequestingUserId)
                throw new AccountNotFoundException($"Account {request.Id} not found");

            return _mapper.Map<AccountDto>(account);
        }
    }
}
