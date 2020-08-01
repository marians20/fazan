namespace WordsCrawler
{
    using CSharpFunctionalExtensions;
    using Microsoft.Extensions.DependencyInjection;
    using System.Threading.Tasks;
    using Fazan.Domain.Abstractions;
    using Fazan.Domain.Ioc;
    using Fazan.Infrastructure.Ioc;

    public class Application
    {
        private ServiceProvider serviceProvider;

        public void ConfigureDi()
        {
            var services = new ServiceCollection();

            services.RegisterSqliteWordsRepository(@"Data Source=d:\fazan.sqlite")
                .RegisterWordsService()
                .RegisterCrawlerService();

            serviceProvider = services.BuildServiceProvider();
        }

        public async Task<Result> Run()
        {
            var crawler = serviceProvider.GetService<ICrawler>();
            return await crawler.ReadAllPages();
        }
    }
}
