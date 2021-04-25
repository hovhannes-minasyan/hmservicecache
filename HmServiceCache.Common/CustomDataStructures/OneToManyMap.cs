using HmServiceCache.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HmServiceCache.Common.CustomDataStructures
{
    public class OneToManyMap<TKey, TObj>
    {
        readonly Dictionary<TKey, List<TObj>> keyToObjectMap = new Dictionary<TKey, List<TObj>>();
        readonly Dictionary<TObj, TKey> objectToKeyMap = new Dictionary<TObj, TKey>();

        public void Add(TKey key, TObj obj)
        {
            keyToObjectMap.SafeKey(key).Add(obj);
            objectToKeyMap.Add(obj, key);
        }

        public void Remove(TKey key, Action<TObj> action)
        {
            keyToObjectMap.Remove(key, out List<TObj> items);
            if (items == null)
                return;

            foreach (var item in items)
            {
                action(item);
                objectToKeyMap.Remove(item);
            }
        }

        public List<TObj> GetAll()
        {
            return objectToKeyMap.Keys.ToList();
        }

        public void Clear() 
        {
            keyToObjectMap.Clear();
            objectToKeyMap.Clear();
        }

        public TObj this[int i]
        {
            get { return objectToKeyMap.ElementAt(i).Key; }
        }

        public int Length => objectToKeyMap.Keys.Count;
        public int Count => objectToKeyMap.Keys.Count;
    }
}
