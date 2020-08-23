using System;
using System.Threading.Tasks;

using ConsoleApplication.Properties;

using CSharpFunctionalExtensions;
using Fazan.Infrastructure.Ioc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApplication
{
    public abstract class ApplicationBase
    {
        protected ApplicationBase()
        {
            Configuration = CreateConfiguration();

            // ReSharper disable once VirtualMemberCallInConstructor
            ConfigureDi();
        }

        public IConfiguration Configuration { get; }

        public abstract Task<Result> RunAsync();

        public IConfiguration CreateConfiguration() =>
            new ConfigurationBuilder()
                .AddJsonFile(Resources.SettingsFileName, optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

        public void ExecuteWithServiceCollection(Action<IServiceCollection> func)
        {
            var services = new ServiceCollection();
            func(services);
            ServiceLocator.Provider = services.BuildServiceProvider();
        }

        protected abstract void ConfigureDi();
    }
}
