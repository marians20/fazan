using System.Threading.Tasks;

using ConsoleApplication;

using CSharpFunctionalExtensions;

using Fazan.Domain.Abstractions;
using Fazan.Domain.Ioc;
using Fazan.Infrastructure.Ioc;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using WordsCrawler.Properties;

namespace WordsCrawler
{
    public class Application : ApplicationBase
    {
        public override Task<Result> RunAsync()
        {
            var crawler = ServiceLocator.GetService<ICrawler>();
            return crawler.ReadAllPages();
        }

        protected override void ConfigureDi() =>
            ExecuteWithServiceCollection(services =>
                services.RegisterSqliteWordsRepository(Configuration.GetConnectionString(Resources.SqliteConnectionStringName))
                .RegisterAllDomainServices()
                .RegisterMassTransit(Configuration));
    }
}
