using System.Collections.Generic;
using System.Linq;
using HmServiceCache.Storage.DataStructures;
using HmServiceCache.Storage.Interfaces;

namespace HmServiceCache.Storage.Storages
{
    public class DataStorage : IDataStorage
    {
        private readonly ValueStructure valueStorage = new ValueStructure();
        private readonly ListStructure listStorage = new ListStructure();
        private readonly HashMapStructure hashMapStorage = new HashMapStructure();

        public void AddToHashMap(string key, string hash, object obj, long timestamp)
        {
            hashMapStorage.AddOrUpdate(key, hash, obj, timestamp);
        }

        public void AddToList(string key, object obj, long timestamp)
        {
            listStorage.AddToList(key, obj, timestamp);
        }

        public void AddValue(string key, object value, long timestamp)
        {
            valueStorage.Add(key, value, timestamp);
        }

        public void Empty()
        {
            valueStorage.Empty();
            listStorage.Empty();
            hashMapStorage.Empty();
        }

        public object GetHashValue(string key, string hashKey)
        {
            return hashMapStorage.GetHashValue(key, hashKey);
        }

        public Dictionary<string, object> GetHasMap(string key)
        {
            return hashMapStorage.GetByKey(key).ToDictionary(i => i.Key, i => i.Value);
        }

        public List<object> GetList(string key)
        {
            return listStorage.GetByKey(key).ToList();
        }

        public object GetListValue(string key, int index)
        {
            return listStorage.GetByKey(key).ElementAtOrDefault(index);
        }

        public object GetValue(string key)
        {
            return valueStorage.GetByKey(key);
        }

        public void RemoveFromHashMap(string key, string hash, long timestamp)
        {
            hashMapStorage.RemoveHashValue(key, hash, timestamp);
        }

        public void RemoveHashMap(string key, long timestamp)
        {
            hashMapStorage.RemoveKey(key, timestamp);
        }

        public void RemoveList(string key, long timestamp)
        {
            listStorage.RemoveKey(key, timestamp);
        }

        public void RemoveValue(string key, long timestamp)
        {
            valueStorage.RemoveKey(key, timestamp);
        }
    }
}
