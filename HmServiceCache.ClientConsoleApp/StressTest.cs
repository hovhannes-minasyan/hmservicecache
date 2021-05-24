using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HmServiceCache.Client.Abstractions;
using HmServiceCache.ClientConsoleApp.Models;

namespace HmServiceCache.ClientConsoleApp
{
    public static class StressTest
    {
        static IHmServiceCache cache;
        
        public static async Task StartAsync(IHmServiceCache hmServiceCache) 
        {
            cache = hmServiceCache;
            //await TestListsSmallData();
            await TestListMediumParallelData();
            await TestListMediumData();
            await TestListLargeData();
        }

        private static async Task TestListsSmallData(int retryCount = 5, int callNum = 1000) 
        {
            var listName = Guid.NewGuid().ToString();
            Console.WriteLine("Testing Small");
            await RunParallelTest(retryCount, callNum, "Inserting into list", async ind =>
            {
                await cache.AddToListAsync(listName, ind);
            }, "Testing Lists With Small Data");

            await RunParallelTest(retryCount, callNum, "Getting from list", async ind =>
            {
                await cache.GetListValueAsync<int>(listName, ind);
            }, "Testing Lists With Small Data");

            await cache.RemoveListAsync(listName);
        }

        private static async Task TestListLargeData(int retryCount = 5, int callNum = 5)
        {
            var largeData = new LargeItemModel
            {
                LargeString = new string('c', 10000),
                LargeLongArray = Enumerable.Range(0, 1000000).Select(a => (long)a).ToArray()
            };

            var listName = Guid.NewGuid().ToString();
            Console.WriteLine($"Large. Size {largeData.GetSize()}");

            await RunParallelTest(retryCount, callNum, "Inserting into list", async ind =>
            {
                await cache.AddToListAsync(listName, largeData);
            }, "Lists With Large Data");

            await RunParallelTest(retryCount, callNum, "Getting from list", async ind =>
            {
                await cache.GetListValueAsync<LargeItemModel>(listName, ind);
            }, "Lists With Large Data");

            await cache.RemoveListAsync(listName);
        }

        private static async Task TestListMediumData(int retryCount = 5, int callNum = 500)
        {
            var medium = new LargeItemModel
            {
                LargeString = new string('c', 1000),
                LargeLongArray = Enumerable.Range(0, 100).Select(a => (long)a).ToArray()
            };

            var listName = Guid.NewGuid().ToString();
            Console.WriteLine($"Testing Medium. Size {medium.GetSize()}");

            await RunParallelTest(retryCount, callNum, "Inserting into list", async ind =>
            {
                await cache.AddToListAsync(listName, medium);
            }, "Testing Lists With Medium Data");

            await RunParallelTest(retryCount, callNum, "Getting from list", async ind =>
            {
                await cache.GetListValueAsync<LargeItemModel>(listName, ind);
            }, "Testing Lists With Medium Data");

            await cache.RemoveListAsync(listName);
        }

        private static async Task TestListMediumParallelData(int retryCount = 5, int callNum = 500)
        {
            var medium = new LargeItemModel
            {
                LargeString = new string('c', 10000),
                LargeLongArray = Enumerable.Range(0, 10000).Select(a => (long)a).ToArray()
            };

            var listName = Guid.NewGuid().ToString();

            Console.WriteLine($"Testing Parallel. Size {medium.GetSize()}");

            await RunParallelTest(retryCount, callNum, "Inserting into list", async ind =>
            {
                await cache.AddToListAsync(listName, medium);
            }, "Testing Lists With Medium Data");

            await RunParallelTest(retryCount, callNum, "Getting from list", async ind =>
            {
                await cache.GetListValueAsync<LargeItemModel>(listName, ind);
            }, "Testing Lists With Medium Data");

            await cache.RemoveListAsync(listName);
        }

        private static async Task RunGenericTest(int retryCount, int callNum, string operation, Func<int, Task> action, string title = "Testing next feature") 
        {
            Console.WriteLine(title);

            var stopwatch = new Stopwatch();
            for (int j = 0; j < retryCount; j++)
            {
                var miliseconds = new List<long>(callNum);
                for (int i = 0; i < callNum; i++)
                {
                    stopwatch.Start();
                    await action(i);
                    stopwatch.Stop();
                    miliseconds.Add(stopwatch.ElapsedMilliseconds);
                    stopwatch.Reset();
                }

                Console.WriteLine($"{operation}: Number of tries: {callNum}. MaxTime: {miliseconds.Max()} MinTime: {miliseconds.Min()} TotalTime: {miliseconds.Sum()}ms");
            }

            Console.WriteLine();
        }

        private static async Task RunParallelTest(int retryCount, int callNum, string operation, Func<int, Task> action, string title = "Testing next feature") 
        {
            var stopwatch = new Stopwatch();
            var tasks = new Task[callNum];
            for (int j = 0; j < retryCount; j++)
            {
                stopwatch.Start();
                for (int i = 0; i < callNum; i++)
                {
                    tasks[i] = action(i);
                }
                Task.WaitAll(tasks);
                stopwatch.Stop();

                Console.WriteLine($"{operation}: AverageTime: {stopwatch.ElapsedMilliseconds / callNum}ms TotalTime: {stopwatch.ElapsedMilliseconds}ms");
                stopwatch.Reset();
            }
        }
    }
}
