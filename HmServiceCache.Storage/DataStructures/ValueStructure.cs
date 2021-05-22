namespace HmServiceCache.Storage.DataStructures
{
    public class ValueStructure : BaseDataStructure<object>
    {
        public void Add(string key, object obj, long timeStamp) => UpdateData(() => collection[key] = obj, timeStamp, key);
    }
}
