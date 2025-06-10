using MediatR;
using SOA.DTOs;
using SOA.Entities;
using SOA.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOA.Commands
{
    public class RegisterUserCommand : IRequest<UserDto>
    {
        public Guid TenantId { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserDto>
    {
        private readonly IUserRepository _userRepository;
       // private readonly ITenantRepository _tenantRepository;
       // private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEventPublisher _eventPublisher;

        public RegisterUserCommandHandler(
            IUserRepository userRepository,
           //ITenantRepository tenantRepository,
            //IRoleRepository roleRepository,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            IEventPublisher eventPublisher)
        {
            _userRepository = userRepository;
            //_tenantRepository = tenantRepository;
            //_roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _eventPublisher = eventPublisher;
        }

        public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
         //   var tenant = await _tenantRepository.GetByIdAsync(request.TenantId);
         //   if (tenant == null) throw new Exception("Invalid tenant ID");

            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null) throw new Exception("Email already registered");

            var hashedPassword = _passwordHasher.Hash(request.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                TenantId = request.TenantId,
                Email = request.Email,
                PasswordHash = hashedPassword,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

          /*  var defaultRole = await _roleRepository.GetByNameAsync("User");
            if (defaultRole != null)
            {
                user.Roles.Add(new UserRole { UserId = user.Id, RoleId = defaultRole.Id });
            }*/

            await _userRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();

            await _eventPublisher.PublishAsync(new { user.Id, user.Email }, "user.registered");

            return new UserDto(user.Id, user.Email, user.IsActive, user.CreatedAt);
        }
    }
    /*
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserDto>
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

        public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                TenantId = request.TenantId,
                Email = request.Email,
                PasswordHash = _passwordHasher.Hash(request.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();

            return new UserDto(user.Id, user.Email, user.IsActive, user.CreatedAt);
        }
    }*/
}
