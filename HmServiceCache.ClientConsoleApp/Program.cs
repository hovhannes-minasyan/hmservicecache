using HmServiceCache.Client.Abstractions;
using HmServiceCache.Client.Extensions;
using HmServiceCache.Client.Models;
using HmServiceCache.Client.RetryPolicies;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace HmServiceCache.ClientConsoleApp
{
    class Program
    {
        static IHmServiceCache cache;
        static void Main(string[] args)
        {
            Console.WriteLine("Press enter to start");
            Console.ReadLine();

            TestMasterConnection().Wait();
            //RunAppAsync().Wait();

            Console.WriteLine("END OF PROCESS");
            Console.WriteLine("Press enter to close");
            Console.ReadLine();
        }

        static async Task TestMasterConnection() 
        {

            var retryPolicy = new ForeverRetryPolicy(TimeSpan.FromMilliseconds(100));
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:15000" + $"/nodehub", opt =>
                {
                    opt.Headers.Add("AccessUri", "http://localhost:15000");
                    opt.Headers.Add("Id", Guid.NewGuid().ToString());
                })
                .WithAutomaticReconnect(retryPolicy)
                .Build();
            await connection.StartAsync();
        }

        private async static Task RunAppAsync()
        {
            Console.WriteLine("App Started");
            cache = StartCache();
            Console.WriteLine("Cache created");
            await Task.Delay(5000);

            await TestListAsync();
            
        }

        private static IHmServiceCache StartCache()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json", true, true).Build();
            var configModel = new ConfigurationModel(configuration["MasterUri"]);
            
            var services = new ServiceCollection().AddHmServiceCache(configModel);

            var provider = services.BuildServiceProvider();
            var cache = provider.GetRequiredService<IHmServiceCache>();
            return cache;
        }

        private static async Task TestListAsync() 
        {
            var listKey = "myList";
            await cache.AddToListAsync(listKey, 5);
            await cache.AddToListAsync(listKey, 6);
            await cache.AddToListAsync(listKey, 7);

            var list = await cache.GetListAsync<int>(listKey);

            Console.WriteLine("Printing received list");
            foreach (var item in list)
            {
                Console.Write(item + " ");
            }
            Console.WriteLine();

            var tempModel = new TempModel
            {
                A = 5,
                B = "Valod",
            };

            var listKey1 = "objList";
            await cache.AddToListAsync(listKey1, tempModel);
            await cache.AddToListAsync(listKey1, tempModel);
            await cache.AddToListAsync(listKey1, tempModel);
            await cache.AddToListAsync(listKey1, tempModel);

            var objList = await cache.GetListAsync<TempModel>(listKey1);
            foreach (var item in objList)
            {
                Console.Write(item + " ");
            }
            Console.WriteLine();
        }
        class TempModel
        {
            public int A { get; set; }
            public string B { get; set; }

            public override string ToString()
            {
                return "{" + $"A = {A} B = {B}" + "}";
            }
        }
    }
}
