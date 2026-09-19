using Microsoft.EntityFrameworkCore;
using Peace_BankingAPI.Domain.Entities;
using Peace_BankingAPI.Domain.Interfaces;
using Peace_BankingAPI.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peace_BankingAPI.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Transaction?> GetByIdAsync(Guid id)
        {
            return await _context.Transactions
                .Include(t => t.SourceAccount)
                .Include(t => t.DestinationAccount)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Transaction?> GetByReferenceAsync(string reference)
        {
            return await _context.Transactions
                .Include(t => t.SourceAccount)
                .Include(t => t.DestinationAccount)
                .FirstOrDefaultAsync(t => t.Reference == reference);
        }

        public async Task<IEnumerable<Transaction>> GetByAccountIdAsync(Guid accountId)
        {
            return await _context.Transactions
                .Where(t => t.SourceAccountId == accountId || t.DestinationAccountId == accountId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByAccountNumberAsync(string accountNumber)
        {
            return await _context.Transactions
                .Include(t => t.SourceAccount)
                .Include(t => t.DestinationAccount)
                .Where(t => t.SourceAccount.AccountNumber == accountNumber
                         || t.DestinationAccount!.AccountNumber == accountNumber)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
        }

        public Task UpdateAsync(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            return Task.CompletedTask;
        }
    }
}
