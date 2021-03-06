﻿using Fazan.Domain.Ioc;

using Microsoft.Extensions.Configuration;

namespace Fazan.Infrastructure.Ioc
{
    using Domain.Abstractions;
    using Logging;
    using Repositories.SqliteRepository;
    using MassTransit;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using WordsRepository = Repositories.SqliteRepository.WordsRepository;

    public static class Ioc
    {
        public static IServiceCollection RegisterLogging(this IServiceCollection services) =>
            services.AddLogging(
                cfg =>
                    {
                    });

        public static IServiceCollection RegisterPlainTextFileWordsRepository(this IServiceCollection services, string destinationPath) =>
            services.AddTransient<IWordsRepository>(s => new Repositories.PlainTextFile.WordsRepository(destinationPath));

        public static IServiceCollection RegisterSqliteWordsRepository(this IServiceCollection services, string connectionString) =>
            services.AddDbContext<FazanSqliteDbContext>(options => options.UseSqlite(connectionString))
                .AddTransient<IWordsRepository, WordsRepository>();

        public static IServiceCollection RegisterMassTransit(this IServiceCollection services, IConfiguration configuration) =>
            services.AddMassTransit(x =>
                {
                    //x.UsingInMemory((context, cfg) =>
                    //{
                    //    cfg.TransportConcurrencyLimit = 100;
                    //    cfg.ConfigureEndpoints(context);
                    //});
                    x.UsingRabbitMq(
                        (context, cfg) =>
                            {
                                cfg.Host(
                                    configuration["RabbitMQ:host"],
                                    configurator =>
                                        {
                                            configurator.Username(configuration["RabbitMQ:user"]);
                                            configurator.Password(configuration["RabbitMQ:password"]);
                                        });
                            });
                })
                .AddTransient(srv => Bus.Factory.CreateMediator(cfg =>
                {
                    cfg.Consumer<LoggingConsumer>();
                }));

        public static IServiceCollection RegisterDomainServices(this IServiceCollection services) =>
            services.RegisterAllDomainServices();
    }
}
