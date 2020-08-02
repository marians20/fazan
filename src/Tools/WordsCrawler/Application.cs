using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Fazan.Application.Common;
using Fazan.Domain.Abstractions;
using Fazan.Domain.Ioc;
using Fazan.Infrastructure.Ioc;
using Microsoft.Extensions.DependencyInjection;

namespace WordsCrawler
{
    public class Application : IApplication
    {
        public void ConfigureDi()
        {
            var services = new ServiceCollection();

            services.RegisterSqliteWordsRepository(@"Data Source=d:\fazan.sqlite")
                .RegisterAllDomainServices()
                .RegisterMassTransit();

            ServiceLocator.Provider = services.BuildServiceProvider();
        }

        public Task<Result> Run()
        {
            var crawler = ServiceLocator.GetService<ICrawler>();
            return crawler.ReadAllPages();
        }
    }
}
