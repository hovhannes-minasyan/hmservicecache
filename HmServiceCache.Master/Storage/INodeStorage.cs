using HmServiceCache.Common.NodeModel;
using HmServiceCache.Master.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace HmServiceCache.Master.Storage
{
    public interface INodeStorage
    {
        public void Add(NodeModel model);
        public ICollection<NodeModel> GetAllNodes();
        public ICollection<HttpClient> GetAllClients();
        public bool Remove(Guid id);
    }
}
