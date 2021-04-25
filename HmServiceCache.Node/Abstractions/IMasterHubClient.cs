using System.Threading.Tasks;

namespace HmServiceCache.Node.Abstractions
{
    public interface IMasterHubClient
    {
        Task StartAsync();
    }
}
