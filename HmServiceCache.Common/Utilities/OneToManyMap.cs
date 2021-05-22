using System;
using System.Collections.Generic;
using System.Linq;
using HmServiceCache.Common.Extensions;

namespace HmServiceCache.Common.Utilities
{
    public class OneToManyMap<TKey, TObj>
    {
        private readonly Dictionary<TObj, TKey> objectToKeyMap = new Dictionary<TObj, TKey>();
        private readonly Dictionary<TKey, List<TObj>> keyToObjectMap = new Dictionary<TKey, List<TObj>>();

        public void Add(TKey key, TObj obj)
        {
            keyToObjectMap.SafeKey(key).Add(obj);
            objectToKeyMap.Add(obj, key);
        }

        public void Remove(TKey key, Action<TObj> action = null)
        {
            keyToObjectMap.Remove(key, out List<TObj> items);
            if (items == null)
                return;

            foreach (var item in items)
            {
                if (action != null)
                {
                    action(item);
                }
                objectToKeyMap.Remove(item);
            }
        }

        public void Remove(TObj obj)
        {
            objectToKeyMap.Remove(obj, out TKey key);
            keyToObjectMap[key].Remove(obj);
        }

        public void Clear()
        {
            keyToObjectMap.Clear();
            objectToKeyMap.Clear();
        }

        public IEnumerable<TObj> GetByKey(TKey key) => keyToObjectMap.GetValueOrDefault(key).AsEnumerable();
        public List<TObj> GetAll() => objectToKeyMap.Keys.ToList();
        public TObj this[int i] => objectToKeyMap.ElementAt(i).Key;
        public int Length => objectToKeyMap.Keys.Count;
        public int Count => objectToKeyMap.Keys.Count;
    }
}
