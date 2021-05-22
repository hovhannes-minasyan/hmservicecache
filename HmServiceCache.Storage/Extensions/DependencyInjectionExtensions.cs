using HmServiceCache.Storage.Interfaces;
using HmServiceCache.Storage.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace HmServiceCache.Storage.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddDataStorages(this IServiceCollection services)
        {
            return services
                .AddSingleton<IDataStorage, DataStorage>()
                .AddSingleton<IDataStorageReader>(p => p.GetService<IDataStorage>())
                .AddSingleton<IDataStorageWriter>(p => p.GetService<IDataStorage>());
        }
    }
}
