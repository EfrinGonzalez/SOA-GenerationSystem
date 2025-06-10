using MediatR;
using SOA.DomainEvents;
using SOA.Entities;
using SOA.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOA.Commands
{
    public class CreateUserCommand : IRequest<Guid>
    {
        public Guid TenantId { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }


    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEventPublisher _publisher;

        public CreateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IEventPublisher publisher)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _publisher = publisher;
        }

        public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var existing = await _userRepository.GetByEmailAsync(request.Email);
            if (existing is not null)
                throw new InvalidOperationException("Email is already in use.");

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

            //await _publisher.PublishAsync(new { user.Id, user.Email }, "user.created");

            await _publisher.PublishAsync(new UserCreatedByAdmin(user.Id, user.Email), "user.created");

            return user.Id;
        }
    }

    //TODO: Maybe the events is a good idea to have somewhere else
    //public record UserCreatedEvent(Guid UserId, string Email);
}
