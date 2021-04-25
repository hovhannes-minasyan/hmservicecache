using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HmServiceCache.Client.Abstractions
{
    public interface IStorageDataWriter
    {
        Task SetValueAsync(string key, object value);
        Task RemoveValueAsync(string key);

        Task AddToListAsync(string key, object value);
        Task RemoveListAsync(string key);

        Task AddToHashMapAsync(string key, string hashKey, object obj);
        Task RemoveHashValueAsync(string key, string hash);
        Task RemoveHashMapAsync(string key);

    }
}
