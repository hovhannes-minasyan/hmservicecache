using System;
using System.Threading.Tasks;
using HmServiceCache.Client.Abstractions;
using HmServiceCache.Client.Models;
using HmServiceCache.Common.CustomDataStructures;
using HmServiceCache.Common.NodeModel;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace HmServiceCache.Client.Services
{
    public class CacheConnectionPool : ICacheConnectionPool
    {
        private int currentIndex;
        private readonly OneToManyMap<Guid, HubConnection> connections = new OneToManyMap<Guid, HubConnection>();
        private readonly ConfigurationModel configuration;

        public CacheConnectionPool(ConfigurationModel configuration)
        {
            this.configuration = configuration;
        }

        public HubConnection Next()
        {
            lock (connections)
            {
                Console.WriteLine("Taking connection at {0}", currentIndex);
                var result = connections[currentIndex++];
                currentIndex = currentIndex % configuration.PoolSize;
                return result;
            }
        }

        public async Task PopulatePoolAsync(NodeModel[] nodeModels)
        {
            for (var i = 0; i < configuration.PoolSize; i++)
            {
                var connection = new HubConnectionBuilder()
                    .WithUrl(nodeModels[i % nodeModels.Length].Url + "/cache")
                    .AddMessagePackProtocol()
                    .Build();

                Console.WriteLine("Trying to connect index {0}", i);
                try
                {
                    await connection.StartAsync();
                    Console.WriteLine("Connection started at {0}", i);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine("Connection failed at {0}", i);
                }
                var id = await connection.InvokeAsync<Guid>("GetId");
                connections.Add(id, connection);
            }
        }
    }
}
