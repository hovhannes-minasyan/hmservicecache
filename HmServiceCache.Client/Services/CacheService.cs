using HmServiceCache.Client.Abstractions;
using HmServiceCache.Client.Models;
using HmServiceCache.Client.RetryPolicies;
using HmServiceCache.Common.NodeModel;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HmServiceCache.Client.Services
{
    internal class CacheService : IHmServiceCache
    {
        private readonly HubConnection masterConnection;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ICacheConnectionPool cacheConnectionPool;

        public CacheService(ConfigurationModel configuration, IHttpClientFactory httpClientFactory, ICacheConnectionPool cacheConnectionPool)
        {
            var retryPolicy = new ForeverRetryPolicy(configuration.RetryIntervalMiliseconds);

            masterConnection = new HubConnectionBuilder()
                .WithUrl(configuration.MasterCacheUrl + "/clienthub")
                .WithAutomaticReconnect(retryPolicy)
                .Build();

            masterConnection.On<NodeModel[]>("LoadCaches", async (nodeModels) =>
            {
                await cacheConnectionPool.PopulatePoolAsync(nodeModels);
            });

            masterConnection.On<NodeModel>("CacheDisconnected", (nodeModel) =>
            {
                Console.WriteLine("Cache disconnected {0}", nodeModel);
            });

            masterConnection.On<NodeModel>("CacheConnected", (nodeModel) =>
            {
                Console.WriteLine("Cache connected {0}", nodeModel);
            });

            masterConnection.StartAsync().Wait();
            this.httpClientFactory = httpClientFactory;
            this.cacheConnectionPool = cacheConnectionPool;
        }

        public Task AddToHashMapAsync(string key, string hashKey, object value)
        {
            var path = GetPath(key, "hashmap", hashKey);
            return MakeRequestAsync(HttpMethod.Put, path, value);
        }

        public Task AddToListAsync(string key, object value)
        {
            var path = GetPath(key, "list");
            return MakeRequestAsync(HttpMethod.Put, path, value);
        }

        public Task<T> GetHashValueAsync<T>(string key, string hashKey)
        {
            return Node.InvokeAsync<T>("GetHashValue", key, hashKey);
        }

        public Task<Dictionary<string, T>> GetHasMapAsync<T>(string key)
        {
            return Node.InvokeAsync<Dictionary<string, T>>("GetHasMap", key);
        }

        public Task<List<T>> GetListAsync<T>(string key)
        {
            return Node.InvokeAsync<List<T>>("GetList", key);
        }

        public Task<T> GetListValueAsync<T>(string key, int index)
        {
            return Node.InvokeAsync<T>("GetListValue", key, index);
        }

        public Task<T> GetValueAsync<T>(string key)
        {
            return Node.InvokeAsync<T>("GetValue", key);
        }

        public Task RemoveHashMapAsync(string key)
        {
            var path = GetPath(key, "hashmap");
            return MakeRequestAsync(HttpMethod.Delete, path);
        }

        public Task RemoveHashValueAsync(string key, string hash)
        {
            var path = GetPath(key, "hashmap", hash);
            return MakeRequestAsync(HttpMethod.Delete, path);
        }

        public Task RemoveListAsync(string key)
        {
            var path = GetPath(key, "list");
            return MakeRequestAsync(HttpMethod.Delete, path);
        }

        public Task RemoveValueAsync(string key)
        {
            var path = GetPath(key, "value");
            return MakeRequestAsync(HttpMethod.Delete, path);
        }

        public Task SetValueAsync(string key, object value)
        {
            var path = GetPath(key, "value");
            return MakeRequestAsync(HttpMethod.Put, path, value);
        }

        private string GetPath(string key, string structure, params string[] parameters)
        {
            var init = $"api/data/{key}/{structure}";
            foreach (var p in parameters)
            {
                init += "/" + p;
            }
            return init;
        }

        private async Task<HttpResponseMessage> MakeRequestAsync(HttpMethod method, string url, object value = null)
        {
            var client = httpClientFactory.CreateClient("HmCacheMaster");
            var json = JsonConvert.SerializeObject(value);
            var content = value == null ? null : new StringContent(json, Encoding.UTF8, "application/json");
            var responseTask = method.Method switch
            {
                "POST" => client.PostAsync(url, content),
                "PUT" => client.PutAsync(url, content),
                "DELETE" => client.DeleteAsync(url),
                "GET" => client.GetAsync(url),
                _ => throw new InvalidOperationException(),
            };
            var response = await responseTask;
            response.EnsureSuccessStatusCode();
            return response;
        }

        private HubConnection Node => cacheConnectionPool.Next();
    }
}
