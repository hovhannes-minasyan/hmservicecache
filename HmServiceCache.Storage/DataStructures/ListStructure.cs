using System.Collections.Generic;
using HmServiceCache.Common.Extensions;

namespace HmServiceCache.Storage.DataStructures
{
    public class ListStructure : BaseDataStructure<List<object>>
    {
        public void AddToList(string key, object obj, long timeStamp) => UpdateData(() => collection.SafeKey(key).Add(obj), timeStamp, key);
    }
}
