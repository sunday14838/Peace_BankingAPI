using MediatR;
using Peace_BankingAPI.Application.DTOs.Auth;
using Peace_BankingAPI.Application.Features.Auth.Commands;
using Peace_BankingAPI.Domain.Exceptions;
using Peace_BankingAPI.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peace_BankingAPI.Application.Features.Auth.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWork _unitOfWork;

        public LoginCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtService jwtService,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var email = request.Email.ToLowerInvariant();

            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
            {
                throw new DomainException("User does not exist");
            }

            var verifyPassword = _passwordHasher.Verify(request.Password, user.PasswordHash);

            if (!verifyPassword)
            {
                throw new DomainException("Invalid email or password");
            }

            if (!user.IsActive)
                throw new DomainException("Account is disabled. Contact support.");

            user.LastLoginAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Generate the JWT
            var (token, expiresAt) = _jwtService.GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                ExpiresAt = expiresAt,
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString()
            };
        }
    }
}
