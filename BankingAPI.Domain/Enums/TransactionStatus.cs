using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peace_BankingAPI.Domain.Enums
{
    public enum TransactionStatus
    {
        Pending = 1,    
        Completed = 2,  
        Failed = 3,      
        Reversed = 4,  
        Blocked = 5 
    }
}
