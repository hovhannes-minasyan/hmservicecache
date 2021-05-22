using System;
using System.Linq;
using System.Threading.Tasks;
using HmServiceCache.Master.Storage;
using Microsoft.AspNetCore.SignalR;

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
            await Clients.Caller.LoadCaches(nodeStorage.GetAllNodes().Select(a => a.Url).ToArray());
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
