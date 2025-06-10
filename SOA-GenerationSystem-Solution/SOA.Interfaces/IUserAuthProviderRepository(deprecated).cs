using SOA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOA.Interfaces
{
    public interface IUserAuthProviderRepository
    {
        Task AddAsync(UserAuthProvider provider);
        Task<UserAuthProvider?> GetByProviderAndExternalIdAsync(string providerName, string externalUserId);
        Task<IEnumerable<UserAuthProvider>> GetByUserIdAsync(Guid userId);
    }
}
