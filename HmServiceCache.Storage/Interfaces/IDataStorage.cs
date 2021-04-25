namespace HmServiceCache.Storage.Interfaces
{
    public interface IDataStorage : IDataStorageWriter, IDataStorageReader
    {
        void Empty();
    }
}
