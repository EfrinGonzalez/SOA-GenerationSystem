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
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context) => _context = context;

        public async Task<User?> GetByIdAsync(Guid id) => await _context.Users.FindAsync(id);

        public async Task<User?> GetByEmailAsync(string email) => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<IEnumerable<User>> GetAllAsync() => await _context.Users.ToListAsync();

        public async Task<IEnumerable<User>> GetByTenantIdAsync(Guid tenantId) =>
            await _context.Users.Where(u => u.TenantId == tenantId).ToListAsync();

        public async Task<IEnumerable<User>> GetByRoleIdAsync(Guid roleId) =>
            await _context.Users.Where(u => u.Roles.Any(r => r.RoleId == roleId)).ToListAsync();

        public async Task AddAsync(User user) => await _context.Users.AddAsync(user);

        public Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null) _context.Users.Remove(user);
        }
    }
}
