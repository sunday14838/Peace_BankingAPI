using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peace_BankingAPI.Domain.Exceptions
{
    public class InsufficientFundsException : DomainException
    {
        public InsufficientFundsException(string message) : base(message) { }
    }
}
