namespace Fazan.Infrastructure.Ioc
{
    using Fazan.Domain.Abstractions;
    using Fazan.Infrastructure.Repositories.SqliteRepository;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using WordsRepository = Repositories.SqliteRepository.WordsRepository;

    public static class Ioc
    {
        public static IServiceCollection RegisterPlainTextFileWordsRepository(this IServiceCollection services, string destinationPath) =>
            services.AddTransient<IWordsRepository>(s => new Repositories.PlainTextFile.WordsRepository(destinationPath));

        public static IServiceCollection RegisterSqliteWordsRepository(this IServiceCollection services, string connectionString) =>
            services.AddDbContext<FazanSqliteDbContext>(options => options.UseSqlite(connectionString))
                .AddTransient<IWordsRepository, WordsRepository>();
    }
}
