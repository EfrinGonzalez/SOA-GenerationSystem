using SOA.Entities;
using SOA.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOA.Factories
{
    public class UserFactory : IUserFactory
    {
        private readonly IPasswordHasher _passwordHasher;

        public UserFactory(IPasswordHasher passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public User Create(Guid tenantId, string email, string password)
        {
            return new User
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Email = email,
                PasswordHash = _passwordHasher.Hash(password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
