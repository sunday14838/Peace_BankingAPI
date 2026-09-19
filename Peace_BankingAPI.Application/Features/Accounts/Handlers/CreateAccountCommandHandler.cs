using AutoMapper;
using MediatR;
using Peace_BankingAPI.Application.DTOs.Account;
using Peace_BankingAPI.Application.Features.Accounts.Commands;
using Peace_BankingAPI.Domain.Entities;
using Peace_BankingAPI.Domain.Enums;
using Peace_BankingAPI.Domain.Exceptions;
using Peace_BankingAPI.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Peace_BankingAPI.Application.Features.Accounts.Handlers
{
    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, AccountDto>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

      
        public CreateAccountCommandHandler(
            IAccountRepository accountRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<AccountDto> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            // 1. Validation
            if (string.IsNullOrEmpty(request.AccountName))
                throw new DomainException("Account name is required");

            // 2. Generate account number (we'll add logic for this)
            var accountNumber = GenerateAccountNumber();

            // 3. Create the account entity
            var account = new Account
            {
                Id = Guid.NewGuid(),
                AccountNumber = accountNumber,
                AccountName = request.AccountName,
                UserId = request.UserId,
                Balance = 0, 
                Currency = Enum.Parse<Currency>(request.Currency),
                Type = Enum.Parse<AccountType>(request.AccountType),
                Status = AccountStatus.Active,
                DailyWithdrawalLimit = 5000,
                MonthlyTransferLimit = 20000,
                CreatedAt = DateTime.UtcNow
            };

            await _accountRepository.AddAsync(account);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

           
            return _mapper.Map<AccountDto>(account);
        }

        private string GenerateAccountNumber()
        {
            string bankCode = "101"; // Your bank/branch prefix (3 digits)

            // 1. Generate a unique 6-digit serial number
            string serialNumber = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

            // 2. Calculate the check digit based on CBN NUBAN standards
            string checkDigit = CalculateNubanCheckDigit(bankCode, serialNumber);

            // 3. Return the full 10-digit account number
            return $"{bankCode}{serialNumber}{checkDigit}";
        }

        private string CalculateNubanCheckDigit(string bankCode, string serialNumber)
        {
            // The official NUBAN algorithm uses multipliers: 3, 7, 3, 3, 7, 3, 3, 7, 3
            int[] multipliers = { 3, 7, 3, 3, 7, 3, 3, 7, 3 };
            string combined = bankCode + serialNumber; // 9 digits total

            int totalSum = 0;
            for (int i = 0; i < 9; i++)
            {
                int digit = int.Parse(combined[i].ToString());
                totalSum += digit * multipliers[i];
            }

            // Calculate modulo 10 check digit
            int modulo = totalSum % 10;
            int checkDigit = 10 - modulo;

            if (checkDigit == 10)
                checkDigit = 0;

            return checkDigit.ToString();
        }
    }
}
