using System;
using System.Linq;
using System.Threading.Tasks;
using HmServiceCache.Common.NodeModel;
using HmServiceCache.Master.Constants;
using HmServiceCache.Master.Storage;
using HmServiceCache.Storage.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace HmServiceCache.Master.Hubs
{
    public class NodeServiceHub : Hub<NodeServiceProxy>
    {
        private readonly INodeStorage nodeStorage;
        private readonly IDataStorageReader dataStorage;
        private readonly IHubContext<ClientServiceHub, IClientServiceProxy> clientHubContext;

        public NodeServiceHub(IHubContext<ClientServiceHub, IClientServiceProxy> clientHubContext, INodeStorage nodeStorage, IDataStorageReader dataStorage)
        {
            this.clientHubContext = clientHubContext;
            this.nodeStorage = nodeStorage;
            this.dataStorage = dataStorage;
        }

        public async override Task OnConnectedAsync()
        {
            foreach (var i in Context.GetHttpContext().Request.Headers["Id"])
            {
                Console.WriteLine($"ID = {i}");
            }

            await MasterLocks.ConnectionLock.AcquireWriterLock();

            var id = Guid.Parse(Context.GetHttpContext().Request.Headers["Id"].First());
            var accessUrl = Context.GetHttpContext().Request.Headers["AccessUri"].First();

            var storageModel = new NodeModel()
            {
                Id = id,
                Url = Context.GetHttpContext().Request.Headers["AccessUriInternal"].First(),
            };

            await Clients.Caller.GetInitialState(dataStorage.GetAll());

            nodeStorage.Add(storageModel);
            MasterLocks.ConnectionLock.ReleaseWriterLock();

            await clientHubContext.Clients.All.CacheConnected(accessUrl);
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            var id = Guid.Parse(Context.GetHttpContext().Request.Headers["Id"]);
            await clientHubContext.Clients.All.CacheDisconnected(id);
            await base.OnDisconnectedAsync(exception);
            nodeStorage.Remove(id);
        }
    }
}
