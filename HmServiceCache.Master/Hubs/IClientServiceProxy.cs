using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HmServiceCache.Common.NodeModel;

namespace HmServiceCache.Master.Hubs
{
    public interface IClientServiceProxy
    {
        Task CacheDisconnected(Guid id);
        Task CacheConnected(NodeModel nodeModel);
        Task LoadCaches(ICollection<NodeModel> model);
    }
}
