using Microsoft.EntityFrameworkCore;
using SOA.Entities;
using SOA.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOA.Persistence
{
    public class UserAuthProviderRepository : IUserAuthProviderRepository
    {
        private readonly AppDbContext _context;
        public UserAuthProviderRepository(AppDbContext context) => _context = context;

        public async Task AddAsync(UserAuthProvider provider) => await _context.UserAuthProviders.AddAsync(provider);

        public async Task<UserAuthProvider?> GetByProviderAndExternalIdAsync(string providerName, string externalUserId) =>
            await _context.UserAuthProviders.FirstOrDefaultAsync(p => p.ProviderName == providerName && p.ExternalUserId == externalUserId);

        public async Task<IEnumerable<UserAuthProvider>> GetByUserIdAsync(Guid userId) =>
            await _context.UserAuthProviders.Where(p => p.UserId == userId).ToListAsync();
    }
}
