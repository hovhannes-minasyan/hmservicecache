using System.Collections.Generic;
using HmServiceCache.Common.Extensions;

namespace HmServiceCache.Storage.DataStructures
{
    public class HashMapStructure : BaseDataStructure<Dictionary<string, object>>
    {
        public void AddOrUpdate(string key, string hash, object obj, long timeStamp) => UpdateData(() => collection.SafeKey(key)[hash] = obj, timeStamp, key);

        public void RemoveHashValue(string key, string hash, long timestamp) => UpdateData(() => collection.SafeKey(key).Remove(hash, out _), timestamp, key);

        public object GetHashValue(string key, string hash) => GetSomeData(() => collection.SafeKey(key)[hash], key);
    }
}
