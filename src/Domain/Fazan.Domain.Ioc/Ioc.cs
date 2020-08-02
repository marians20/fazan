namespace Fazan.Domain.Ioc
{
    using Abstractions;
    using FluentValidation;
    using Microsoft.Extensions.DependencyInjection;
    using Models;
    using Services.CrawlerService;
    using Services.DomProcessorService;
    using Services.Validators;
    using Services.WordsServices;

    public static class Ioc
    {
        public static IServiceCollection RegisterDomProcessor(this IServiceCollection services) =>
            services.AddTransient<IDomProcessor, DomProcessor>();

        public static IServiceCollection RegisterCrawlerService(this IServiceCollection services) =>
            services.AddHttpClient()
                .RegisterDomProcessor()
                .AddTransient<ICrawlerContext, CrawlerContext>()
                .AddTransient<ICrawler, Crawler>();

        public static IServiceCollection RegisterWordsService(this IServiceCollection services) =>
            services.RegisterValidators()
                .AddTransient<IWordsService, WordsService>();

        public static IServiceCollection RegisterValidators(this IServiceCollection services) =>
            services.AddTransient<AbstractValidator<Word>, WordValidator>();

        public static IServiceCollection RegisterAllDomainServices(this IServiceCollection services) =>
            services.RegisterDomProcessor()
                .RegisterCrawlerService()
                .RegisterWordsService()
                .RegisterValidators();
    }
}
