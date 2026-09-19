using MediatR;
using Peace_BankingAPI.Application.DTOs.Transaction;
using Peace_BankingAPI.Application.Features.Transactions.Commands;
using Peace_BankingAPI.Domain.Entities;
using Peace_BankingAPI.Domain.Enums;
using Peace_BankingAPI.Domain.Exceptions;
using Peace_BankingAPI.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Peace_BankingAPI.Application.Features.Transactions.Handlers
{
    public class TransferCommandHandler : IRequestHandler<TransferCommand, TransferResultDto>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TransferCommandHandler(
            IAccountRepository accountRepository,
            ITransactionRepository transactionRepository,
            IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
        }


        public async Task<TransferResultDto> Handle(TransferCommand request, CancellationToken cancellationToken)
        {
            // Basic input validation (before touching the database)

            if (request.Amount <= 0)
                throw new DomainException("Transfer amount must be greater than zero");

            if (string.IsNullOrWhiteSpace(request.FromAccountNumber))
                throw new DomainException("Source account number is required");

            if (string.IsNullOrWhiteSpace(request.ToAccountNumber))
                throw new DomainException("Destination account number is required");

            if (request.FromAccountNumber == request.ToAccountNumber)
                throw new DomainException("Cannot transfer to the same account");

            // Begin the database transaction
            // This ensures ALL-or-NOTHING. If anything fails, we roll back.
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                // Fetch source and destination accounts
                var sourceAccount = await _accountRepository.GetByAccountNumberAsync(request.FromAccountNumber);
                if (sourceAccount == null)
                    throw new AccountNotFoundException($"Source account {request.FromAccountNumber} not found");

                var destinationAccount = await _accountRepository.GetByAccountNumberAsync(request.ToAccountNumber);
                if (destinationAccount == null)
                    throw new AccountNotFoundException($"Destination account {request.ToAccountNumber} not found");

                if (sourceAccount.Status != AccountStatus.Active)
                    throw new DomainException($"Source account is {sourceAccount.Status} and cannot send money");

                if (destinationAccount.Status != AccountStatus.Active)
                    throw new DomainException($"Destination account is {destinationAccount.Status} and cannot receive money");

                if (sourceAccount.Currency != destinationAccount.Currency)
                    throw new DomainException("Cannot transfer between accounts with different currencies");

                if (Enum.Parse<Currency>(request.Currency) != sourceAccount.Currency)
                    throw new DomainException("Transfer currency does not match account currency");

                if (sourceAccount.Balance < request.Amount)
                    throw new InsufficientFundsException(
                        $"Insufficient balance. Available: {sourceAccount.Balance:F2} {sourceAccount.Currency}, " +
                        $"Requested: {request.Amount:F2} {sourceAccount.Currency}");

                if (request.Amount > sourceAccount.DailyWithdrawalLimit)
                    throw new DomainException(
                        $"Transfer exceeds daily limit of {sourceAccount.DailyWithdrawalLimit:F2}");

                sourceAccount.Withdraw(request.Amount);
                destinationAccount.Deposit(request.Amount);


                var reference = GenerateReference();

                var transaction = new Transaction
                {
                    Id = Guid.NewGuid(),
                    SourceAccountId = sourceAccount.Id,
                    DestinationAccountId = destinationAccount.Id,
                    Amount = request.Amount,
                    Currency = sourceAccount.Currency,
                    Type = TransactionType.Transfer,
                    Status = TransactionStatus.Completed,
                    Reference = reference,
                    Narration = request.Narration ?? $"Transfer from {sourceAccount.AccountNumber} to {destinationAccount.AccountNumber}",
                    CompletedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                // Persist everything
                await _transactionRepository.AddAsync(transaction);
                await _accountRepository.UpdateAsync(sourceAccount);
                await _accountRepository.UpdateAsync(destinationAccount);

                // One SaveChanges = one database round-trip = atomic
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Commit the transaction
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                // Return the result
                return new TransferResultDto
                {
                    Success = true,
                    TransactionId = transaction.Id,
                    Reference = reference,
                    Amount = request.Amount,
                    Currency = sourceAccount.Currency.ToString(),
                    FromAccountNumber = sourceAccount.AccountNumber,
                    ToAccountNumber = destinationAccount.AccountNumber,
                    FromAccountNewBalance = sourceAccount.Balance,
                    ToAccountNewBalance = destinationAccount.Balance,
                    Status = TransactionStatus.Completed.ToString(),
                    Timestamp = DateTime.UtcNow,
                    Narration = request.Narration,
                    Message = "Transfer completed successfully"
                };
            }
            catch
            {
                // On ANY failure, roll everything back
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw; 
            }
        }

        // Generate a unique, human-readable reference
        private static string GenerateReference()
        {
            // Format: TXN + date + random
            // Example: TXN20260115A3F9B2
            return $"TXN{DateTime.UtcNow:yyyyMMdd}{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
        }
    }

}
