using Peace_BankingAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peace_BankingAPI.Domain.Interfaces
{
    public interface IJwtService
    {
        (string Token, DateTime ExpiresAt) GenerateToken(User user);
    }
}
