using MediatR;
using SOA.DomainEvents;
using SOA.Interfaces;

namespace SOA.Commands
{
    public class UpdateUserCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventPublisher _eventPublisher;

        public UpdateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IEventPublisher eventPublisher)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _eventPublisher = eventPublisher;
        }

        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null) return false;

            user.Email = request.Email;
            user.IsActive = request.IsActive;
            user.CreatedAt = DateTime.UtcNow; 

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.CommitAsync();
            await _eventPublisher.PublishAsync(new UserUpdated(user.Id, user.Email ,user.IsActive), "user.updated");

            return true;
        }
    }
}
