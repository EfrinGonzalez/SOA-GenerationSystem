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
    public class GetUsersByTenantIdQuery : IRequest<IEnumerable<UserDto>>
    {
        public Guid TenantId { get; set; }
    }

    public class GetUsersByTenantIdQueryHandler : IRequestHandler<GetUsersByTenantIdQuery, IEnumerable<UserDto>>
    {
        private readonly IUserRepository _userRepository;

        public GetUsersByTenantIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserDto>> Handle(GetUsersByTenantIdQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetByTenantIdAsync(request.TenantId);
            return users.Select(u => new UserDto(u.Id, u.Email, u.IsActive, u.CreatedAt));
        }
    }
}
