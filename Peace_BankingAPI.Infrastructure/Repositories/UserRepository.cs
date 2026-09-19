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
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
           
            var normalized = email.Trim().ToLowerInvariant();

            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == normalized);
        }

        public async Task<User?> GetByPhoneNumberAsync(string phoneNumber)
        {
            var normalized = phoneNumber.Trim();

            return await _context.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == normalized);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var normalized = email.Trim().ToLowerInvariant();

            return await _context.Users
                .AnyAsync(u => u.Email.ToLower() == normalized);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            return Task.CompletedTask;
        }
    }
}
