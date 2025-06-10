using SOA.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOA.Infrastructure
{
    public class RabbitMqEventPublisher : IEventPublisher
    {
        public Task PublishAsync<T>(T @event, string topic)
        {
            // TODO: Implement actual RabbitMQ publishing
            Console.WriteLine($"[RabbitMQ] Topic: {topic} | Event: {@event}");
            return Task.CompletedTask;
        }
    }
}
