using MassTransit;
using System;

namespace Fazan.Infrastructure.Logging
{
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;

    public class LoggingConsumer : IConsumer<Log>
    {
        public Task Consume(ConsumeContext<Log> context) => Task.Run(() => Console.WriteLine(context.Message));
    }

    public class Log
    {
        public static Log Create(string message, LogLevel logLevel = LogLevel.Information) => new Log(message, logLevel);

        private Log(string message, LogLevel logLevel = LogLevel.Information)
        {
            Message = message;
            LogLevel = logLevel;
        }

        public string Message { get; }

        public LogLevel LogLevel { get; }

        public override string ToString() => $"{DateTime.Now:G}|{Enum.GetName(typeof(LogLevel), LogLevel)}|{Message}";
    }
}
