using System.Collections.Generic;
using System.Threading.Tasks;

namespace HmServiceCache.Node.Abstractions
{
    public interface IDataStorageAsyncReader
    {
        Task<object> GetValueAsync(string key);

        Task<List<object>> GetListAsync(string key);
        Task<object> GetListValueAsync(string key, int index);

        Task<Dictionary<string, object>> GetHasMapAsync(string key);
        Task<object> GetHashValueAsync(string key, string hashKey);
    }
}
