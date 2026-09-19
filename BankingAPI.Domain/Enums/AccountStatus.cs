using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peace_BankingAPI.Domain.Enums
{
    public enum AccountStatus
    {
        Pending = 1,
        Active = 2,
        Frozen = 3,
        Closed = 4,
        Blocked = 5
    }
}
