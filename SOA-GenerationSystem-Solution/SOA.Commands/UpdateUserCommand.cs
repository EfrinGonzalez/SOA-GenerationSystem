using MediatR;
using SOA.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOA.Commands
{
    public class UpdateUserCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public bool? IsActive { get; set; }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventPublisher _publisher;

        public UpdateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IEventPublisher publisher)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _publisher = publisher;
        }

        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null) return false;

            if (request.Email != null)
                user.Email = request.Email;
            if (request.IsActive.HasValue)
                user.IsActive = request.IsActive.Value;

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.CommitAsync();

            await _publisher.PublishAsync(new UserUpdatedEvent(user.Id, user.Email), "user.updated");
            return true;
        }
    }

    public record UserUpdatedEvent(Guid UserId, string Email);
}
