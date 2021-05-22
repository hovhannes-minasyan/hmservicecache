using System;

namespace HmServiceCache.Common.NodeModel
{
    public class NodeModel
    {
        public Guid Id { get; set; }
        public string AccessUrl { get; set; }
        public string InternalAccessUrl { get; set; }

        public override string ToString()
        {
            return $"Id ={ Id }  Url = {InternalAccessUrl}";
        }
    }
}
