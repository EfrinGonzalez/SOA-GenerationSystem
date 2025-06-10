using MediatR;
using SOA.DTOs;
using SOA.Interfaces;

namespace SOA.Queries
{
    public class GetAllUsersQuery : IRequest<IEnumerable<UserDto>> { }

   

    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
    {
        private readonly IUserRepository _userRepository;

        public GetAllUsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => new UserDto(u.Id, u.Email, u.IsActive, u.CreatedAt));
        }
    }
}
