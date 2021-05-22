using System.Threading.Tasks;
using HmServiceCache.Common.NodeModel;
using Microsoft.AspNetCore.SignalR.Client;

namespace HmServiceCache.Client.Abstractions
{
    public interface ICacheConnectionPool
    {
        HubConnection Next();
        Task PopulatePoolAsync(NodeModel[] nodeModels);
    }
}
