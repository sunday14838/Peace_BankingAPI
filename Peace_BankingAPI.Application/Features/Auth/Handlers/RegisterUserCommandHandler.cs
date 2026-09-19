using MediatR;
using Peace_BankingAPI.Application.DTOs.Auth;
using Peace_BankingAPI.Application.Features.Auth.Commands;
using Peace_BankingAPI.Domain.Entities;
using Peace_BankingAPI.Domain.Enums;
using Peace_BankingAPI.Domain.Exceptions;
using Peace_BankingAPI.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peace_BankingAPI.Application.Features.Auth.Handlers
{
    public class RegisterUserCommandHandler
    : IRequestHandler<RegisterUserCommand, RegisterUserResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task<RegisterUserResponseDto> Handle(
            RegisterUserCommand request,
            CancellationToken cancellationToken)
        {

            // Normalize input
            var email = request.Email.Trim().ToLowerInvariant();
            var phoneNumber = request.PhoneNumber.Trim();
            var firstName = request.FirstName.Trim();
            var lastName = request.LastName.Trim();

            if (await _userRepository.EmailExistsAsync(email))
                throw new DomainException("A user with this email already exists");

            var existingByPhone = await _userRepository.GetByPhoneNumberAsync(phoneNumber);
            if (existingByPhone != null)
                throw new DomainException("A user with this phone number already exists");

            var passwordHash = _passwordHasher.Hash(request.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                PhoneNumber = phoneNumber,
                FirstName = firstName,
                LastName = lastName,
                PasswordHash = passwordHash,
                Role = UserRole.Customer,  
                IsActive = true,
                EmailConfirmed = false,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new RegisterUserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
                CreatedAt = user.CreatedAt
            };
        }
    }
}
