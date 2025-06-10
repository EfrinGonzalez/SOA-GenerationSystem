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
    public class AddAuthProviderCommand : IRequest<Guid>
    {
        public Guid UserId { get; set; }
        public string ProviderName { get; set; } = null!;
        public string ExternalUserId { get; set; } = null!;
    }

    public class AddAuthProviderCommandHandler : IRequestHandler<AddAuthProviderCommand, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddAuthProviderCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(AddAuthProviderCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null) throw new ArgumentException("User not found");

            var provider = new UserAuthProvider
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                ProviderName = request.ProviderName,
                ExternalUserId = request.ExternalUserId,
                CreatedAt = DateTime.UtcNow
            };

            // We'll assume repository handles auth providers for now
            if (_userRepository is IUserAuthProviderRepository providerRepo)
            {
                await providerRepo.AddAsync(provider);
            }

            await _unitOfWork.CommitAsync();
            return provider.Id;
        }
    }
}
