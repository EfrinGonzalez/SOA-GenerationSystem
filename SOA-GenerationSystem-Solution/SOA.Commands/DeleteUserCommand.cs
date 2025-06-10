using MediatR;
using SOA.DomainEvents;
using SOA.Interfaces;

namespace SOA.Commands
{
    public class DeleteUserCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventPublisher _eventPublisher;

        public DeleteUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IEventPublisher eventPublisher)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _eventPublisher = eventPublisher;
        }

        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            await _userRepository.DeleteAsync(request.UserId);
            await _unitOfWork.CommitAsync();
            await _eventPublisher.PublishAsync(new UserDeleted(request.UserId), "user.deleted");
            return true;
        }
    }
}
