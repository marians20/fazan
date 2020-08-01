using System;

namespace PlayGame
{
    using CSharpFunctionalExtensions;
    using Fazan.Domain.Ioc;
    using Fazan.Infrastructure.Ioc;
    using Microsoft.Extensions.DependencyInjection;
    using System.Threading.Tasks;
    using Fazan.Domain.Abstractions;
    using Fazan.Domain.Services;
    using Fazan.Infrastructure.Logging;
    using MassTransit.Mediator;

    public class Application
    {
        private ServiceProvider serviceProvider;

        public void ConfigureDi()
        {
            IServiceCollection services = new ServiceCollection();

            services.RegisterSqliteWordsRepository(@"Data Source=d:\fazan.sqlite")
                .RegisterWordsService()
                .RegisterMassTransit();

            serviceProvider = services.BuildServiceProvider();
        }

        public async Task<Result> Run()
        {
            var words = serviceProvider.GetService<IWordsService>();
            var mediator = serviceProvider.GetService<IMediator>();

            string playersWord;
            string computersWord = "";

            do
            {
                await mediator.Send(Log.Create("Enter a word"));
                playersWord = Console.ReadLine();

                await words.GetHardestWord(playersWord.Substring(playersWord.Length - Constants.LettersCount))
                    .Tap(async word =>
                    {
                        computersWord = word;
                        await mediator.Send(Log.Create($"- {computersWord}"));
                    })
                    .OnFailure(async error =>  await mediator.Send(Log.Create("- M-ai ars! :(")));


            } while (!string.IsNullOrEmpty(playersWord) && !string.IsNullOrEmpty(computersWord));

            return Result.Success();
        }
    }
}
