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
    public class LinkAuthProviderCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public string ProviderName { get; set; } = null!;
        public string ExternalUserId { get; set; } = null!;
    }

    public class LinkAuthProviderCommandHandler : IRequestHandler<LinkAuthProviderCommand, bool>
    {
        private readonly IUserAuthProviderRepository _authRepo;
        private readonly IUnitOfWork _unitOfWork;

        public LinkAuthProviderCommandHandler(
            IUserAuthProviderRepository authRepo,
            IUnitOfWork unitOfWork)
        {
            _authRepo = authRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(LinkAuthProviderCommand request, CancellationToken cancellationToken)
        {
            var entity = new UserAuthProvider
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                ProviderName = request.ProviderName,
                ExternalUserId = request.ExternalUserId,
                CreatedAt = DateTime.UtcNow
            };

            await _authRepo.AddAsync(entity);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
