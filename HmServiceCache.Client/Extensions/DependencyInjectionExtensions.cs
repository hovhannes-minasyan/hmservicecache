using HmServiceCache.Client.Abstractions;
using HmServiceCache.Client.Models;
using HmServiceCache.Client.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HmServiceCache.Client.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddHmServiceCache(this IServiceCollection services, ConfigurationModel configuration)
        {
            services.AddSingleton(p => configuration);
            services.AddHttpClient("HmCacheMaster", c => { c.BaseAddress = new Uri(configuration.MasterCacheUrl); });
            services.AddSingleton<IHmServiceCache, CacheService>();
            services.AddSingleton<ICacheConnectionPool, CacheConnectionPool>();
            return services;
        }
    }
}
