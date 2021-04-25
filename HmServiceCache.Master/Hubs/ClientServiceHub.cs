using HmServiceCache.Master.Storage;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace HmServiceCache.Master.Hubs
{
    public class ClientServiceHub : Hub<IClientServiceProxy>
    {
        private readonly INodeStorage nodeStorage;

        public ClientServiceHub(INodeStorage nodeStorage)
        {
            this.nodeStorage = nodeStorage;
        }

        public async override Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await Clients.Caller.LoadCaches(nodeStorage.GetAllNodes());
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
