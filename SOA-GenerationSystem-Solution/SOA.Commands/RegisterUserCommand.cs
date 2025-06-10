using MediatR;
using SOA.DomainEvents;
using SOA.DTOs;
using SOA.Interfaces;


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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEventPublisher _eventPublisher;
        private readonly IUserFactory _userFactory;


        public RegisterUserCommandHandler(
            IUserRepository userRepository,     
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            IEventPublisher eventPublisher,
            IUserFactory userFactory)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _eventPublisher = eventPublisher;
            _userFactory = userFactory;
        }

        public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {        
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null) throw new Exception("Email already registered");

            //var hashedPassword = _passwordHasher.Hash(request.Password);

            var user = _userFactory.Create(request.TenantId, request.Email, request.Password);            

            await _userRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();
            await _eventPublisher.PublishAsync(new UserSelfRegistered(user.Id, user.Email), "user.registered");

            return new UserDto(user.Id, user.Email, user.IsActive, user.CreatedAt);
        }
    }   
}
