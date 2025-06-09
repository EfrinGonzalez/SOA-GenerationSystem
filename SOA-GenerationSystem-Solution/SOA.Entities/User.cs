using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOA.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<UserRole> Roles { get; set; } = new List<UserRole>();
    }
}
