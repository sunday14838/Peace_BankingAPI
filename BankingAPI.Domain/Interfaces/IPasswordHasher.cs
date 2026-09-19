using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peace_BankingAPI.Domain.Interfaces
{
    public interface IPasswordHasher
    {
        string Hash(string plainPassword);
        bool Verify(string plainPassword, string storedHash);
    }
}
