using System.Collections.Generic;
using System.Linq;
using HmServiceCache.Common.Extensions;

namespace HmServiceCache.Storage.DataStructures
{
    public class ListStructure : BaseDataStructure<List<object>>
    {
        public void AddToList(string key, object obj, long timeStamp) => UpdateData(() => collection.SafeKey(key).Add(obj), timeStamp, key);
        public object ElementAt(string key, int index) => SafeRetreiveData(() => collection.SafeKey(key).ElementAtOrDefault(index), key);
    }
}
