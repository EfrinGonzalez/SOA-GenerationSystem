using SOA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOA.Interfaces
{
    public interface IUserFactory
    {
        User Create(Guid tenantId, string email, string password);
    }
}
