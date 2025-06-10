using MediatR;
using SOA.Entities;
using SOA.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOA.Commands
{
    public class AssignRoleToUserCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }

    public class AssignRoleToUserCommandHandler : IRequestHandler<AssignRoleToUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AssignRoleToUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null) return false;

            if (!user.Roles.Any(r => r.RoleId == request.RoleId))
            {
                user.Roles.Add(new UserRole { UserId = request.UserId, RoleId = request.RoleId });
                await _userRepository.UpdateAsync(user);
                await _unitOfWork.CommitAsync();
            }

            return true;
        }
    }
}
