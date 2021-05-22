using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using HmServiceCache.Common.NodeModel;

namespace HmServiceCache.Master.Storage
{
    internal class NodeStorage : INodeStorage
    {

        private readonly Dictionary<Guid, HttpClient> httpClients = new Dictionary<Guid, HttpClient>();
        private readonly Dictionary<Guid, NodeModel> nodes = new Dictionary<Guid, NodeModel>();
        private readonly IHttpClientFactory clientFactory;
        private static readonly object lockObject = new object();

        public NodeStorage(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public void Add(NodeModel model)
        {
            lock (lockObject)
            {
                nodes.TryAdd(model.Id, model);
                var client = clientFactory.CreateClient();
                client.BaseAddress = new Uri(nodes[model.Id].InternalAccessUrl);
                httpClients.TryAdd(model.Id, client);
            }
        }

        public ICollection<NodeModel> GetAllNodes()
        {
            lock (lockObject)
            {
                return nodes.Values.ToArray();
            }

        }

        public ICollection<HttpClient> GetAllClients()
        {
            lock (lockObject)
            {
                return httpClients.Values.ToArray();
            }
        }

        public bool Remove(Guid id)
        {
            lock (lockObject)
            {
                return nodes.Remove(id);
            }
        }
    }
}
