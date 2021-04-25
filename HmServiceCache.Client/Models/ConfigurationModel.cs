namespace HmServiceCache.Client.Models
{
    public class ConfigurationModel
    {
        public string MasterCacheUrl { get; }
        public int RetryIntervalMiliseconds { get; set; }
        public int PoolSize { get; set; }

        public ConfigurationModel(string masterCacheUrl)
        {
            MasterCacheUrl = masterCacheUrl;
            RetryIntervalMiliseconds = 100;
            PoolSize = 10;
        }
    }
}
