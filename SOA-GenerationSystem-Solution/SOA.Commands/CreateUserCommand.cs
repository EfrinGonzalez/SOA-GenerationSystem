using MediatR;
using SOA.DomainEvents;
using SOA.Interfaces;

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
        private readonly IUserFactory _userFactory;


        public CreateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IEventPublisher publisher, IUserFactory userFactory)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _publisher = publisher;
            _userFactory = userFactory;
        }

        public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var existing = await _userRepository.GetByEmailAsync(request.Email);
            if (existing is not null)
                throw new InvalidOperationException("Email is already in use.");

            var user = _userFactory.Create(request.TenantId, request.Email, request.Password);

            await _userRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();
            await _publisher.PublishAsync(new UserCreatedByAdmin(user.Id, user.Email), "user.created");

            return user.Id;
        }
    }
}
