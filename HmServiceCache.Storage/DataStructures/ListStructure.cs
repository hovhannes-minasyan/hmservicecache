using HmServiceCache.Common.Extensions;
using System.Collections.Concurrent;

namespace HmServiceCache.Storage.DataStructures
{
    public class ListStructure : BaseDataStructure<ConcurrentBag<object>>
    {
        public void AddToList(string key, object obj, long timeStamp) => UpdateData(() => collection.SafeKey(key).Add(obj), timeStamp, key);
    }
}
