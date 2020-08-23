using System;
using System.Collections.Generic;

using ConsoleApplication;

namespace AppRunner
{
    internal static class ApplicationsFactory
    {
        private static readonly IDictionary<string, Func<ApplicationBase>> Dict =
            new Dictionary<string, Func<ApplicationBase>>
            {
               { "play", () => new PlayGame.Application() },
               { "addwords", () => new WordsCrawler.Application() }
            };

        public static ApplicationBase GetApplication(string applicationName) => Dict[applicationName]();
    }
}
