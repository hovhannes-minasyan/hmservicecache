using System.Collections.Generic;
using MessagePack;

namespace HmServiceCache.Storage.Models
{
    [MessagePackObject]
    public class FullDataStorageModel
    {
        [Key(0)]
        public Dictionary<string, object> Values { get; set; }
        [Key(1)]
        public Dictionary<string, List<object>> Lists { get; set; }
        [Key(2)]
        public Dictionary<string, Dictionary<string, object>> HashMaps { get; set; }
    }
}
