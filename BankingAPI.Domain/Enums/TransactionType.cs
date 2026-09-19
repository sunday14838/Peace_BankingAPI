using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peace_BankingAPI.Domain.Enums
{
    public enum TransactionType
    {
        Deposit = 1,
        Withdrawal = 2,
        Transfer = 3,
        Fee = 4,
        Interest = 5,
        Reversal = 6
    }
}
