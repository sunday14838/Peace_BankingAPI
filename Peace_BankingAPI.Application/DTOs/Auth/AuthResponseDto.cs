using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peace_BankingAPI.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        // The token the client will use for subsequent requests
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }

        // Basic user info the frontend usually needs to display
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
