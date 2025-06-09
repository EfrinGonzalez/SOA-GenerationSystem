using MediatR;
using SOA.DTOs;
using SOA.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOA.Queries
{
    public class GetUsersByRoleIdQuery : IRequest<IEnumerable<UserDto>>
    {
        public Guid RoleId { get; set; }
    }

    public class GetUsersByRoleIdQueryHandler : IRequestHandler<GetUsersByRoleIdQuery, IEnumerable<UserDto>>
    {
        private readonly IUserRepository _userRepository;

        public GetUsersByRoleIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserDto>> Handle(GetUsersByRoleIdQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetByRoleIdAsync(request.RoleId);
            return users.Select(u => new UserDto(u.Id, u.Email, u.IsActive, u.CreatedAt));
        }
    }
}
