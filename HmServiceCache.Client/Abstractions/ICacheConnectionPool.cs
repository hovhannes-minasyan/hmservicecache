using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace HmServiceCache.Client.Abstractions
{
    public interface ICacheConnectionPool
    {
        Task AddConnectionAsync(string url);
        Task<HubConnection> NextAsync();
        Task PopulatePoolAsync(string[] urls);
        Task RemoveConnection(Guid id);
    }
}
