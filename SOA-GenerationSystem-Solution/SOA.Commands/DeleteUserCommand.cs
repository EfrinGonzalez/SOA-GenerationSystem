using MediatR;
using SOA.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOA.Commands
{
    public class DeleteUserCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventPublisher _publisher;

        public DeleteUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IEventPublisher publisher)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _publisher = publisher;
        }

        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null) return false;

            await _userRepository.DeleteAsync(request.Id);
            await _unitOfWork.CommitAsync();

            await _publisher.PublishAsync(new UserDeletedEvent(user.Id, user.Email), "user.deleted");
            return true;
        }
    }

    public record UserDeletedEvent(Guid UserId, string Email);
}
