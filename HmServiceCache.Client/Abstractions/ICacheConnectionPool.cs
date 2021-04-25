using HmServiceCache.Common.NodeModel;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HmServiceCache.Client.Abstractions
{
    public interface ICacheConnectionPool
    {
        HubConnection Next();
        Task PopulatePoolAsync(NodeModel[] nodeModels);
    }
}
