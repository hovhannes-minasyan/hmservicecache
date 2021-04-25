using HmServiceCache.Storage.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HmServiceCache.Node.Controllers
{

    [ApiController]
    [Route("api/data/{key}/")]
    public class DataReceiverController : ControllerBase
    {
        private readonly IDataStorage dataStorage;

        public DataReceiverController(IDataStorage dataStorage)
        {
            this.dataStorage = dataStorage;
        }

        [HttpPut("hashmap/{hash}")]
        public ActionResult AddToHashMap([FromRoute] string key, [FromRoute] string hash, [FromBody] object obj, [FromQuery] long timeStamp)
        {
            dataStorage.AddToHashMap(key, hash, obj, timeStamp);
            return Ok();
        }

        [HttpPut("value")]
        public ActionResult AddValue([FromRoute] string key, [FromBody] object value, [FromQuery] long timeStamp)
        {
            dataStorage.AddValue(key, value, timeStamp);
            return Ok();
        }

        [HttpPut("list")]
        public ActionResult AddToList([FromRoute] string key, [FromBody] object value, [FromQuery] long timeStamp)
        {
            dataStorage.AddToList(key, value, timeStamp);
            return Ok();
        }

        [HttpDelete("hashmap/{hash}")]
        public ActionResult RemoveFromHashMap([FromRoute] string key, [FromRoute] string hash, [FromQuery] long timeStamp)
        {
            dataStorage.RemoveFromHashMap(key, hash, timeStamp);
            return Ok();
        }

        [HttpDelete("hashmap")]
        public ActionResult RemoveHashMap([FromRoute] string key, [FromQuery] long timeStamp)
        {
            dataStorage.RemoveHashMap(key, timeStamp);
            return Ok();
        }


        [HttpDelete("value")]
        public ActionResult RemoveValue([FromRoute] string key, [FromQuery] long timeStamp)
        {
            dataStorage.RemoveValue(key, timeStamp);
            return Ok();
        }

        [HttpDelete("list")]
        public ActionResult RemoveList([FromRoute] string key, [FromQuery] long timeStamp)
        {
            dataStorage.RemoveList(key, timeStamp);
            return Ok();
        }
    }
}
