using System;
using System.Collections;
using System.Collections.Generic;

namespace HmServiceCache.Common.Extensions
{
    public static class DictionaryExtensions
    {
        public static V SafeKey<T,V>(this IDictionary<T, V> dictionary, T key) where V : class, IEnumerable, new()
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, new V());
            }

            return dictionary[key];
        }
    }
}
