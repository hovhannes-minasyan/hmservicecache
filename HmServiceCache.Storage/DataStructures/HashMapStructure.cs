using HmServiceCache.Common.Extensions;
using System.Collections.Concurrent;

namespace HmServiceCache.Storage.DataStructures
{
    public class HashMapStructure : BaseDataStructure<ConcurrentDictionary<string, object>>
    {
        public void AddOrUpdate(string key, string hash, object obj, long timeStamp) => UpdateData(() => collection.SafeKey(key)[hash] = obj, timeStamp, key);

        public void RemoveHashValue(string key, string hash, long timestamp) => UpdateData(() => collection.SafeKey(key).TryRemove(hash, out _), timestamp, key);

        public object GetHashValue(string key, string hash) => GetSomeData(() => collection.SafeKey(key)[hash], key);
    }
}
