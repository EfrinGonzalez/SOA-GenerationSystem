﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOA.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T @event, string topic);
    }
}
