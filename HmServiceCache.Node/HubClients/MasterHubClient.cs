using System;
using System.Threading.Tasks;
using HmServiceCache.Client.RetryPolicies;
using HmServiceCache.Node.Abstractions;
using HmServiceCache.Node.Hubs;
using HmServiceCache.Node.Models;
using HmServiceCache.Storage.Interfaces;
using HmServiceCache.Storage.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HmServiceCache.Node.HubClients
{
    public class MasterHubClient : IMasterHubClient
    {
        private readonly HubConnection connection;

        public MasterHubClient(IConfiguration configuration, IDataStorageWriter storage, IHubContext<CachingHub, ICachingClientProxy> clientHub, ConfigurationModel config)
        {
            var masterUri = Environment.GetEnvironmentVariable("MasterUri") ?? configuration["MasterUri"];
            var accessUri = Environment.GetEnvironmentVariable("AccessUri") ?? configuration["AccessUri"];
            var internalUri = Environment.GetEnvironmentVariable("AccessUriInternal") ?? accessUri;

            Console.WriteLine("Connection to master {0}", masterUri);

            var retryPolicy = new ForeverRetryPolicy(TimeSpan.FromMilliseconds(int.Parse(configuration["RetryIntervalMiliseconds"])));
            connection = new HubConnectionBuilder()
                .WithUrl(masterUri + $"/nodehub", opt =>
                {
                    opt.Headers.Add("AccessUri", accessUri);
                    opt.Headers.Add("Id", config.Id.ToString());
                    opt.Headers.Add("AccessUriInternal", internalUri);
                })
                .WithAutomaticReconnect(retryPolicy)
                .AddMessagePackProtocol()
                .Build();

            connection.Closed += (e) => { storage.Empty(); return Task.CompletedTask; };
            connection.On("GetInitialState", (FullDataStorageModel data) =>
            {
                storage.SetAll(data);
            });
        }

        public async Task StartAsync()
        {
            await connection.StartAsync();
        }
    }
}
