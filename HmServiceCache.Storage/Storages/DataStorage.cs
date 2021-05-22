using System.Collections.Generic;
using System.Linq;
using HmServiceCache.Storage.DataStructures;
using HmServiceCache.Storage.Interfaces;
using HmServiceCache.Storage.Models;

namespace HmServiceCache.Storage.Storages
{
    public class DataStorage : IDataStorage
    {
        private readonly ValueStructure valueStorage = new ValueStructure();
        private readonly ListStructure listStorage = new ListStructure();
        private readonly HashMapStructure hashMapStorage = new HashMapStructure();

        public void AddToHashMap(string key, string hash, object obj, long timestamp) => hashMapStorage.AddOrUpdate(key, hash, obj, timestamp);

        public void AddToList(string key, object obj, long timestamp) => listStorage.AddToList(key, obj, timestamp);

        public void AddValue(string key, object value, long timestamp) => valueStorage.Add(key, value, timestamp);

        public object GetHashValue(string key, string hashKey) => hashMapStorage.GetHashValue(key, hashKey);

        public Dictionary<string, object> GetHasMap(string key) => hashMapStorage.GetByKey(key).ToDictionary(i => i.Key, i => i.Value);

        public List<object> GetList(string key) => listStorage.GetByKey(key).ToList();

        public object GetListValue(string key, int index) => listStorage.ElementAt(key, index);

        public object GetValue(string key) => valueStorage.GetByKey(key);

        public void RemoveFromHashMap(string key, string hash, long timestamp) => hashMapStorage.RemoveHashValue(key, hash, timestamp);

        public void RemoveHashMap(string key, long timestamp) => hashMapStorage.RemoveKey(key, timestamp);

        public void RemoveList(string key, long timestamp) => listStorage.RemoveKey(key, timestamp);

        public void RemoveValue(string key, long timestamp) => valueStorage.RemoveKey(key, timestamp);

        public FullDataStorageModel GetAll() => new FullDataStorageModel
        {
            Values = valueStorage.GetAll(),
            Lists = listStorage.GetAll(),
            HashMaps = hashMapStorage.GetAll(),
        };

        public void SetAll(FullDataStorageModel fullDataStorageModel)
        {
            valueStorage.Set(fullDataStorageModel.Values);
            listStorage.Set(fullDataStorageModel.Lists);
            hashMapStorage.Set(fullDataStorageModel.HashMaps);
        }

        public void Empty()
        {
            valueStorage.Empty();
            listStorage.Empty();
            hashMapStorage.Empty();
        }
    }
}
