using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ConsoleApplication;

using CSharpFunctionalExtensions;

using Fazan.Domain.Abstractions;
using Fazan.Domain.Ioc;
using Fazan.Domain.Models;
using Fazan.Domain.Services;
using Fazan.Infrastructure.Ioc;
using MassTransit.Mediator;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using PlayGame.Properties;

namespace PlayGame
{
    public class Application : ApplicationBase
    {
        public override async Task<Result> RunAsync()
        {
            var words = ServiceLocator.GetService<IWordsService>();
            var mediator = ServiceLocator.GetService<IMediator>();

            var usedWords = new List<string>();

            string playersWord;
            var computersWord = string.Empty;

            do
            {
                var wordExists = true;
                do
                {
                    await mediator.Send(Log.Create("Enter a word")).ConfigureAwait(false);
                    playersWord = Console.ReadLine().Trim().ToLower();
                    if (usedWords.Contains(playersWord))
                    {
                        await mediator.Send(Log.Create($"Word {playersWord} already used! Try again.")).ConfigureAwait(false);
                        continue;
                    }

                    wordExists = string.IsNullOrEmpty(playersWord) || await words.Exists(playersWord).ConfigureAwait(false);

                    if (!wordExists)
                    {
                        await mediator.Send(Log.Create($"Word {playersWord} does not exist! Try again.")).ConfigureAwait(false);
                        continue;
                    }
                }
                while ((!string.IsNullOrEmpty(playersWord) && usedWords.Contains(playersWord)) || !wordExists);

                if (string.IsNullOrEmpty(playersWord))
                {
                    await mediator.Send(Log.Create("- Te-am ars! :)")).ConfigureAwait(false);
                }

                usedWords.Add(playersWord);

                await ComputerReply(words, playersWord, usedWords).Tap(
                    async word =>
                        {
                            computersWord = word;
                            usedWords.Add(word);
                            await mediator.Send(Log.Create($"- {computersWord}")).ConfigureAwait(false);
                        }).OnFailure(error => mediator.Send(Log.Create("- M-ai ars! :("))).ConfigureAwait(false);
            }
            while (!(string.IsNullOrEmpty(playersWord) || string.IsNullOrEmpty(computersWord)));

            return Result.Success();
        }

        protected override void ConfigureDi()
        {
            IServiceCollection services = new ServiceCollection();

            services.RegisterSqliteWordsRepository(Configuration.GetConnectionString(Resources.SqliteConnectionStringName))
                .RegisterAllDomainServices()
                .RegisterMassTransit(Configuration);

            ServiceLocator.Provider = services.BuildServiceProvider();
        }

        private static Task<Result<string>> ComputerReply(IWordsService words, string playersWord, IList<string> excludedWords) =>
            words.GetAWord(playersWord.Substring(playersWord.Length - Constants.LettersCount), excludedWords);
    }
}
