using MediatR;
using SOA.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOA.Queries
{
    public class GetAuthProvidersByUserQuery : IRequest<IEnumerable<UserAuthProviderDto>>
    {
        public Guid UserId { get; set; }
    }

    public record UserAuthProviderDto(string ProviderName, string ExternalUserId, DateTime CreatedAt);

    public class GetAuthProvidersByUserQueryHandler : IRequestHandler<GetAuthProvidersByUserQuery, IEnumerable<UserAuthProviderDto>>
    {
        private readonly IUserAuthProviderRepository _authRepo;

        public GetAuthProvidersByUserQueryHandler(IUserAuthProviderRepository authRepo)
        {
            _authRepo = authRepo;
        }

        public async Task<IEnumerable<UserAuthProviderDto>> Handle(GetAuthProvidersByUserQuery request, CancellationToken cancellationToken)
        {
            var providers = await _authRepo.GetByUserIdAsync(request.UserId);
            return providers.Select(p => new UserAuthProviderDto(p.ProviderName, p.ExternalUserId, p.CreatedAt));
        }
    }
}
