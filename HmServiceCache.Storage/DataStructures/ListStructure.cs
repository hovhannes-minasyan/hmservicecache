using System.Collections.Concurrent;
using HmServiceCache.Common.Extensions;

namespace HmServiceCache.Storage.DataStructures
{
    public class ListStructure : BaseDataStructure<ConcurrentBag<object>>
    {
        public void AddToList(string key, object obj, long timeStamp) => UpdateData(() => collection.SafeKey(key).Add(obj), timeStamp, key);
    }
}
