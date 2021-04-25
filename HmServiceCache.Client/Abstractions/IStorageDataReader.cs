using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HmServiceCache.Client.Abstractions
{
    public interface IStorageDataReader
    {
        Task<T> GetValueAsync<T>(string key);

        Task<List<T>> GetListAsync<T>(string key);
        Task<T> GetListValueAsync<T>(string key, int index);

        Task<Dictionary<string, T>> GetHasMapAsync<T>(string key);
        Task<T> GetHashValueAsync<T>(string key, string hashKey);
    }
}
