using System.Threading.Tasks;
using Couchbase.Core;

namespace CouchMan
{
    public interface IClusterService
    {
        Task<IClusterInfo> GetClusterInfoAsync();
    }
}