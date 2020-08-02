using System;

using Microsoft.Extensions.DependencyInjection;

namespace Fazan.Infrastructure.Ioc
{
    public static class ServiceLocator
    {
        public static IServiceProvider Provider { get; set; }

        public static T GetService<T>() => Provider.GetService<T>();
    }
}
