using System;
using System.Threading.Tasks;
using HmServiceCache.Client.Abstractions;
using HmServiceCache.Client.Extensions;
using HmServiceCache.Client.Models;
using HmServiceCache.Client.RetryPolicies;
using HmServiceCache.ClientConsoleApp.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HmServiceCache.ClientConsoleApp
{
    internal class Program
    {
        private static IHmServiceCache cache;

        private static void Main(string[] args)
        {
            Console.WriteLine("Press enter to start");
            Console.ReadLine();

            //TestMasterConnection().Wait();
            RunTestAppAsync().GetAwaiter().GetResult();
            //StartCliAsync().GetAwaiter().GetResult();

            Console.WriteLine("END OF PROCESS");
            Console.WriteLine("Press enter to close");
            Console.ReadLine();
        }

        private static async Task TestMasterConnection()
        {
            var retryPolicy = new ForeverRetryPolicy(TimeSpan.FromMilliseconds(100));
            const string url = "http://localhost:15000";

            Console.WriteLine("Testing master connection");
            var connection = new HubConnectionBuilder()
                .WithUrl(url + $"/nodehub", opt =>
                {
                    opt.Headers.Add("AccessUri", url);
                    opt.Headers.Add("AccessUriInternal", url);
                    opt.Headers.Add("Id", Guid.NewGuid().ToString());
                })
                .WithAutomaticReconnect(retryPolicy)
                .AddMessagePackProtocol()
                .Build();

            await connection.StartAsync();

            Console.WriteLine("Test node connected to master");

            Console.WriteLine("Testing master connection");

            var connection1 = new HubConnectionBuilder()
             .WithUrl(url + "/clienthub")
             .AddMessagePackProtocol()
             .WithAutomaticReconnect(retryPolicy)
             .Build();

            await connection1.StartAsync();

            Console.WriteLine("Test client connected to master");
        }

        private async static Task RunTestAppAsync()
        {
            Console.WriteLine("App Started");
            cache = StartCache();
            Console.WriteLine("Cache created");


            await StressTest.StartAsync(cache);
            //await TestListAsync();
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

            for (int i = 0; i < 10; i++)
            {
                var list = await cache.GetListAsync<int>(listKey);

                Console.WriteLine("Printing received list");
                foreach (var item in list)
                {
                    Console.Write(item + " ");
                }
                Console.WriteLine();
            }

            var tempModel = new TempModel
            {
                A = 5,
                B = "Valod",
            };

            var listKey1 = "objList";
            await cache.AddToListAsync(listKey1, tempModel);
            tempModel.A++;
            await cache.AddToListAsync(listKey1, tempModel);
            tempModel.A++;
            await cache.AddToListAsync(listKey1, tempModel);
            tempModel.A++;
            await cache.AddToListAsync(listKey1, tempModel);

            for (int i = 0; i < 10; i++)
            {
                var objList = await cache.GetListAsync<TempModel>(listKey1);
                foreach (var item in objList)
                {
                    Console.Write(item + " ");
                }
                Console.WriteLine();
            }

            Console.WriteLine();
        }

        private static async Task StartCliAsync()
        {
            cache = StartCache();
            while (true)
            {
                Console.Write("HMCache> ");
                var command = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(command))
                    continue;

                var splitted = command.Split(' ');
                var str = splitted[0].ToLower();
                if (str == "quit")
                    break;

                var operation = splitted[1].ToLower();

                if (str == "list")
                {
                    if (operation == "add")
                    {
                        await cache.AddToListAsync(splitted[2], splitted[3]);
                    }
                    else if (operation == "get")
                    {
                        var result = await cache.GetListValueAsync<string>(splitted[2], int.Parse(splitted[3]));
                        Console.WriteLine(result);
                    }
                }
            }
        }
    }
}
