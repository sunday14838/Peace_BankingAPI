using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Peace_BankingAPI.Application.Settings;
using Peace_BankingAPI.Domain.Entities;
using Peace_BankingAPI.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Peace_BankingAPI.Infrastructure.Auth
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _settings;

        public JwtService(IOptions<JwtSettings> options)
        {
            _settings = options.Value;
        }

        public (string Token, DateTime ExpiresAt) GenerateToken(User user)
        {
            var expiresAt = DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes);

            var claims = new List<Claim>
        {
            // "sub" = subject (the user identifier). Standard JWT claim.
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),

            // "jti" = JWT ID. Unique per-token identifier, useful for blacklisting later.
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            // "email" - so services downstream can display the user without a DB hit.
            new(JwtRegisteredClaimNames.Email, user.Email),

            // Custom claims our API will read directly.
            new("userId", user.Id.ToString()),
            new("role", user.Role.ToString()),
            new("firstName", user.FirstName),
            new("lastName", user.LastName)
        };

            // Symmetric key from our shared secret
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiresAt,
                signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return (tokenString, expiresAt);
        }
    }
}
