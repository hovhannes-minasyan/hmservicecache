using HmServiceCache.Common.NodeModel;
using HmServiceCache.Master.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HmServiceCache.Master.Hubs
{
    public interface IClientServiceProxy
    {
        Task CacheDisconnected(Guid id);
        Task CacheConnected(NodeModel nodeModel);
        Task LoadCaches(ICollection<NodeModel> model);
    }
}
