using HmServiceCache.Client.RetryPolicies;
using HmServiceCache.Node.Abstractions;
using HmServiceCache.Node.Hubs;
using HmServiceCache.Node.Models;
using HmServiceCache.Storage.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace HmServiceCache.Node.HubClients
{
    public class MasterHubClient : IMasterHubClient
    {
        private readonly HubConnection connection;
        private readonly IConfiguration configuration;
        private readonly IDataStorage storage;
        private readonly IHubContext<CachingHub, ICachingClientProxy> clientHub;

        public MasterHubClient(IConfiguration configuration, IDataStorage storage, IHubContext<CachingHub, ICachingClientProxy> clientHub, ConfigurationModel config)
        {
            this.configuration = configuration;
            this.storage = storage;
            this.clientHub = clientHub;
            var masterUri = Environment.GetEnvironmentVariable("MasterUri") ?? configuration["MasterUri"];
            var accessUri = Environment.GetEnvironmentVariable("AccessUri")?? configuration["AccessUri"];
            var internalUri = Environment.GetEnvironmentVariable("AccessUriInternal") ?? accessUri;

            Console.WriteLine("Connection to master {0}", masterUri);

            var retryPolicy = new ForeverRetryPolicy(TimeSpan.FromMilliseconds(int.Parse(configuration["RetryIntervalMiliseconds"])));
            connection = new HubConnectionBuilder()
                .WithUrl( masterUri + $"/nodehub", opt =>
                 {
                     opt.Headers.Add("AccessUri", accessUri);
                     opt.Headers.Add("Id", config.Id.ToString());
                     opt.Headers.Add("AccessUriInternal", internalUri);
                 })
                .WithAutomaticReconnect(retryPolicy)
                .Build();

            connection.Closed += (e) => { storage.Empty(); return Task.CompletedTask; };
        }

        public async Task StartAsync()
        {
            await connection.StartAsync();
        }
    }
}
