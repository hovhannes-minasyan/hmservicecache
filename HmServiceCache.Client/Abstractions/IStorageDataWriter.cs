using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HmServiceCache.Client.Abstractions
{
    public interface IStorageDataWriter
    {
        Task SetValueAsync<T>(string key, T value);
        Task RemoveValueAsync(string key);

        Task AddToListAsync<T>(string key, T value);
        Task RemoveListAsync(string key);

        Task AddToHashMapAsync<T>(string key, string hashKey, T obj);
        Task RemoveHashValueAsync(string key, string hash);
        Task RemoveHashMapAsync(string key);

    }
}
