using SOA.DTOs;
using SOA.Entities;
using SOA.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOA.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _hasher;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork, IPasswordHasher hasher)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _hasher = hasher;
        }

        private async Task<User> CreateBaseUserAsync(Guid tenantId, string email, string password)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Email = email,
                PasswordHash = _hasher.Hash(password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();
            return user;
        }

        public async Task<UserDto> RegisterUserAsync(RegisterUserDto dto)
        {
            var existing = await _userRepository.GetByEmailAsync(dto.Email);
            if (existing != null)
                throw new InvalidOperationException("A user with that email already exists.");

            // Default tenant
            var defaultTenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            var user = await CreateBaseUserAsync(defaultTenantId, dto.Email, dto.Password);

            return new UserDto(user.Id, user.Email, user.IsActive, user.CreatedAt);
        }


        public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                TenantId = dto.TenantId,
                Email = dto.Email,
                PasswordHash = _hasher.Hash(dto.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();

            return new UserDto(user.Id, user.Email, user.IsActive, user.CreatedAt);
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => new UserDto(u.Id, u.Email, u.IsActive, u.CreatedAt));
        }

        public async Task<UserDto?> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : new UserDto(user.Id, user.Email, user.IsActive, user.CreatedAt);
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user == null ? null : new UserDto(user.Id, user.Email, user.IsActive, user.CreatedAt);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _userRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
        }
    }

}
