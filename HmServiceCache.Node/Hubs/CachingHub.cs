using HmServiceCache.Node.Abstractions;
using HmServiceCache.Node.Models;
using HmServiceCache.Storage.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HmServiceCache.Node.Hubs
{
    public class CachingHub : Hub<ICachingClientProxy>, IDataStorageReader
    {
        private readonly IDataStorage dataStorage;
        private readonly ConfigurationModel configurationModel;

        public CachingHub(IDataStorage dataStorage, ConfigurationModel configurationModel)
        {
            this.dataStorage = dataStorage;
            this.configurationModel = configurationModel;
        }

        public Guid GetId() 
        {
            return configurationModel.Id;
        }

        public object GetHashValue(string key, string hashKey)
        {
            return dataStorage.GetHashValue(key, hashKey);
        }

        public Dictionary<string, object> GetHasMap(string key)
        {
            return dataStorage.GetHasMap(key);
        }

        public List<object> GetList(string key)
        {
            return dataStorage.GetList(key);
        }

        public object GetListValue(string key, int index)
        {
            return dataStorage.GetListValue(key, index);
        }

        public object GetValue(string key)
        {
            var result = dataStorage.GetValue(key);
            return result;
        }

        public async override Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
