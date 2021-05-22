using System;
using System.Threading.Tasks;
using HmServiceCache.Client.Abstractions;
using HmServiceCache.Client.Models;
using HmServiceCache.Common.Utilities;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace HmServiceCache.Client.Services
{
    public class CacheConnectionPool : ICacheConnectionPool
    {
        private static readonly AsyncReaderWriterLock asyncReaderWriterLock = new AsyncReaderWriterLock();
        private int currentIndex;
        private readonly OneToManyMap<Guid, HubConnection> connections = new OneToManyMap<Guid, HubConnection>();
        private readonly ConfigurationModel configuration;

        public CacheConnectionPool(ConfigurationModel configuration)
        {
            this.configuration = configuration;
        }

        public async Task<HubConnection> NextAsync()
        {

            while (true)
            {
                await asyncReaderWriterLock.AcquireReaderLock();
                try
                {
                    if (connections.Count == 0)
                    {
                        asyncReaderWriterLock.ReleaseReaderLock();
                        await Task.Delay(1000);
                        continue;
                    }
                    Console.WriteLine("Taking connection at {0}", currentIndex);
                    var result = connections[currentIndex++];
                    currentIndex %= connections.Count;
                    return result;
                }
                finally
                {
                    asyncReaderWriterLock.ReleaseReaderLock();
                }

            }

        }

        public async Task PopulatePoolAsync(string[] urls)
        {
            if (urls.Length == 0)
                return;

            await asyncReaderWriterLock.AcquireWriterLock();

            for (var i = 0; i < configuration.PoolSize; i++)
            {
                var connection = await GetConnectionAsync(urls[i % urls.Length]);

                if (connection.State == HubConnectionState.Connected)
                {
                    var id = await connection.InvokeAsync<Guid>("GetId");
                    connections.Add(id, connection);
                }
            }

            asyncReaderWriterLock.ReleaseWriterLock();

        }

        public async Task AddConnectionAsync(string url)
        {
            await asyncReaderWriterLock.AcquireWriterLock();
            Console.WriteLine("Entered add connection lock");

            var isSmall = connections.Count < configuration.PoolSize;
            if (isSmall)
            {
                var connection = await GetConnectionAsync(url);
                var id = await connection.InvokeAsync<Guid>("GetId");
                connections.Add(id, connection);
            }

            asyncReaderWriterLock.ReleaseWriterLock();
            Console.WriteLine("Released add connection lock");
        }

        public async Task RemoveConnection(Guid id)
        {
            await asyncReaderWriterLock.AcquireWriterLock();

            var currentConnections = connections.GetByKey(id);
            connections.Remove(id);

            foreach (var conn in currentConnections)
            {
                await conn.StopAsync();
            }

            asyncReaderWriterLock.ReleaseWriterLock();
        }

        public async Task RemoveConnectionAfterClosing(HubConnection connection)
        {
            await asyncReaderWriterLock.AcquireWriterLock();
            connections.Remove(connection);
            asyncReaderWriterLock.ReleaseWriterLock();
        }

        private async Task<HubConnection> GetConnectionAsync(string url)
        {
            var connection = new HubConnectionBuilder()
                    .WithUrl(url + "/cache")
                    .AddMessagePackProtocol()
                    .Build();

            Console.WriteLine("Trying to connect url {0}", url);

            try
            {
                await connection.StartAsync();
                Console.WriteLine("Connection url at {0}", url);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Connection url at {0}", url);
            }
            return connection;
        }
    }
}
