using HmServiceCache.Master.Storage;
using HmServiceCache.Storage.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Alphacloud.MessagePack.HttpFormatter;
using MessagePack;
using System.Threading;

namespace HmServiceCache.Master.Controllers
{
    [ApiController]
    [Route("api/data/{key}")]
    public class DataController : ControllerBase
    {
        private readonly INodeStorage nodeStorage;
        private readonly IDataStorage dataStorage;

        public DataController(INodeStorage nodeStorage, IDataStorage dataStorage)
        {
            this.nodeStorage = nodeStorage;
            this.dataStorage = dataStorage;
        }

        [HttpGet("test")]
        public ActionResult Test([FromQuery] long time)
        {
            var req = HttpContext.Request;
            Console.WriteLine(req.Path.Value);
            return Ok();
        }

        [HttpPut("hashmap/{hash}")]
        public async Task<ActionResult> AddToHashMap([FromRoute] string key, [FromQuery] string hash, [FromBody] object value)
        {
            var timeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            dataStorage.AddToHashMap(key, hash, value, timeStamp);
            await ForwardRequest(timeStamp, value);

            return Ok();
        }

        [HttpPut("value")]
        public async Task<ActionResult> AddValue([FromRoute] string key, [FromBody] object value)
        {
            var timeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            dataStorage.AddValue(key, value, timeStamp);
            await ForwardRequest(timeStamp, value);

            return Ok();
        }

        [HttpPut("list")]
        public async Task<ActionResult> AddToList([FromRoute] string key, [FromBody] object value)
        {
            var timeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            dataStorage.AddToList(key, value, timeStamp);
            await ForwardRequest(timeStamp, value);

            return Ok();
        }

        [HttpDelete("hashmap/{hash}")]
        public async Task<ActionResult> RemoveFromHashMap([FromRoute] string key, [FromQuery] string hash)
        {
            var timeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            dataStorage.RemoveFromHashMap(key, hash, timeStamp);
            await ForwardRequest(timeStamp);

            return Ok();
        }

        [HttpDelete("hashmap")]
        public async Task<ActionResult> RemoveHashMap([FromRoute] string key)
        {
            var timeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            dataStorage.RemoveHashMap(key, timeStamp);
            await ForwardRequest(timeStamp);

            return Ok();
        }


        [HttpDelete("value")]
        public async Task<ActionResult> RemoveValue([FromRoute] string key)
        {
            var timeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            dataStorage.RemoveValue(key, timeStamp);
            await ForwardRequest(timeStamp);

            return Ok();
        }

        [HttpDelete("list")]
        public async Task<ActionResult> RemoveList([FromRoute] string key)
        {
            var timeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            dataStorage.RemoveList(key, timeStamp);
            await ForwardRequest(timeStamp);

            return Ok();
        }


        private async Task ForwardRequest(long timestamp, object value = null)
        {
            var nodes = nodeStorage.GetAllClients();
            var tasks = new List<Task<HttpResponseMessage>>(nodes.Count);

            var url = $"{HttpContext.Request.Path.Value}?{nameof(timestamp)}={timestamp}";
            var method = HttpContext.Request.Method;
            foreach (var pair in HttpContext.Request.Query)
            {
                url += $"&{pair.Key}={pair.Value}";
            }

            Func<HttpClient, Task<HttpResponseMessage>> createHttpRequestAsync = HttpContext.Request.Method switch
            {
                "POST" => (client) => client.PostAsMsgPackAsync(url, value, CancellationToken.None),
                "PUT" => (client) => client.PutAsMsgPackAsync(url, value, CancellationToken.None),
                "DELETE" => (client) => client.DeleteAsync(url),
                "GET" => (client) => client.GetAsync(url),
                _ => throw new InvalidOperationException(),
            };

            foreach (var client in nodes)
            {
                tasks.Add(createHttpRequestAsync(client));
            }

            foreach (var task in tasks)
            {
                var result = await task;
                if (!result.IsSuccessStatusCode)
                {
                    Console.WriteLine("Faillllllled");
                    // TODO turn off node
                }
            }
        }
    }
}
