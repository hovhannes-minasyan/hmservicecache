using System.Threading.Tasks;
using HmServiceCache.Storage.Models;

namespace HmServiceCache.Master.Hubs
{
    public interface NodeServiceProxy
    {
        Task GetInitialState(FullDataStorageModel fullDataStorageModel);
    }
}
