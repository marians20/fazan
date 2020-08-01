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

    public class Application
    {
        private ServiceProvider serviceProvider;

        public void ConfigureDi()
        {
            IServiceCollection services = new ServiceCollection();

            services.RegisterSqliteWordsRepository(@"Data Source=d:\fazan.sqlite")
                .RegisterWordsService();

            serviceProvider = services.BuildServiceProvider();
        }

        public async Task<Result> Run()
        {
            var words = serviceProvider.GetService<IWordsService>();

            string playersWord;
            string computersWord = "";

            do
            {
                Console.WriteLine("Enter a word");
                playersWord = Console.ReadLine();

                await words.GetHardestWord(playersWord.Substring(playersWord.Length - Constants.LettersCount))
                    .Tap(word =>
                    {
                        computersWord = word;
                        Console.WriteLine($"- {computersWord}");
                    })
                    .OnFailure(error => Console.WriteLine($"- M-ai ars! :("));


            } while (!string.IsNullOrEmpty(playersWord) && !string.IsNullOrEmpty(computersWord));

            return Result.Success();
        }
    }
}
