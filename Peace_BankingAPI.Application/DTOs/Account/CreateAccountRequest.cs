using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peace_BankingAPI.Application.DTOs.Account
{
    public class CreateAccountRequest
    {
        public string AccountName { get; set; } = string.Empty;
        public string Currency { get; set; } = "NGN";
        public string AccountType { get; set; } = "Savings";
    }
}
