using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HmServiceCache.Master.Hubs
{
    public interface IClientServiceProxy
    {
        Task CacheDisconnected(Guid id);
        Task CacheConnected(string url);
        Task LoadCaches(ICollection<string> url);
    }
}
