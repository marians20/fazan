using System;

using MassTransit;

namespace Fazan.Infrastructure.Logging
{
    using System.Threading.Tasks;

    using Domain.Models;

    public class LoggingConsumer : IConsumer<Log>
    {
        public Task Consume(ConsumeContext<Log> context) => Task.Run(() => Console.WriteLine(context.Message));
    }
}
