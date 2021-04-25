using System;

namespace HmServiceCache.Node.Models
{
    public class ConfigurationModel
    {
        public Guid Id { get; }

        public ConfigurationModel()
        {
            Id = Guid.NewGuid();
        }
    }
}
