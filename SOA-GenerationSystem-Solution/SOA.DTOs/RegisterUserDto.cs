﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOA.DTOs
{
    public record RegisterUserDto(Guid TenantId, string Email, string Password);
}
