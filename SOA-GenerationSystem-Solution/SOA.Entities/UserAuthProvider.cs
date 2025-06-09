using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOA.Entities
{
    public class UserAuthProvider
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string ProviderName { get; set; } = null!;
        public string ExternalUserId { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
