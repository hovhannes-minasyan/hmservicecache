namespace HmServiceCache.Storage.Interfaces
{
    public interface IDataStorageWriter
    {
        void AddValue(string key, object value, long timestamp);
        void AddToList(string key, object obj, long timestamp);
        void AddToHashMap(string key, string hash, object obj, long timestamp);

        void RemoveValue(string key, long timestamp);
        void RemoveList(string key, long timestamp);
        void RemoveHashMap(string key, long timestamp);
        void RemoveFromHashMap(string key, string hash, long timestamp);
    }
}
