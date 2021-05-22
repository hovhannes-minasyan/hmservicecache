using System;
using MessagePack;

namespace HmServiceCache.Common.NodeModel
{
    [MessagePackObject]
    public class NodeModel
    {
        [Key(0)]
        public Guid Id { get; set; }
        [Key(1)]
        public string Url { get; set; }

        public override string ToString()
        {
            return $"Id ={ Id }  Url = {Url}";
        }
    }
}
