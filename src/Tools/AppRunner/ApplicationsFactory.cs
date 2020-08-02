using System;
using System.Collections.Generic;
using Fazan.Application.Common;

namespace AppRunner
{
    internal static class ApplicationsFactory
    {
        private static readonly IDictionary<string, Func<IApplication>> Dict =
            new Dictionary<string, Func<IApplication>>
            {
               { "play", () => new PlayGame.Application() },
               { "addwords", () => new WordsCrawler.Application() }
            };

        public static IApplication GetApplication(string applicationName) => Dict[applicationName]();
    }
}
