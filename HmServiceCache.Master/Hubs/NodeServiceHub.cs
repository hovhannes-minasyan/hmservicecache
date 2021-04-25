using HmServiceCache.Common.NodeModel;
using HmServiceCache.Master.Models;
using HmServiceCache.Master.Storage;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HmServiceCache.Master.Hubs
{
    public class NodeServiceHub : Hub<NodeServiceProxy>
    {
        private readonly IHubContext<ClientServiceHub, IClientServiceProxy> clientHubContext;
        private readonly INodeStorage nodeStorage;

        public NodeServiceHub(IHubContext<ClientServiceHub, IClientServiceProxy> clientHubContext, INodeStorage nodeStorage)
        {
            this.clientHubContext = clientHubContext;
            this.nodeStorage = nodeStorage;
        }

        public async override Task OnConnectedAsync()
        {
            foreach(var i in Context.GetHttpContext().Request.Headers["Id"]) 
            {
                Console.WriteLine($"ID = {i}");
            }

            var id = Guid.Parse(Context.GetHttpContext().Request.Headers["Id"].First());
            var clientModel = new NodeModel()
            {
                Id = id,
                Url = Context.GetHttpContext().Request.Headers["AccessUri"].First(),
            };
            var storageModel = new NodeModel()
            {
                Id = id,
                Url = Context.GetHttpContext().Request.Headers["AccessUriInternal"].First(),
            };
            
            await base.OnConnectedAsync();
            nodeStorage.Add(storageModel);
            await clientHubContext.Clients.All.CacheConnected(clientModel);
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            var id = Guid.Parse(Context.GetHttpContext().Request.Query["Id"]);
            await clientHubContext.Clients.All.CacheDisconnected(id);
            await base.OnDisconnectedAsync(exception);
            nodeStorage.Remove(id);
        }
    }
}
