using System.Threading.Tasks;
using Couchbase.Core;

namespace Couchmon.Couchbase
{
    public interface IClusterService
    {
        Task<IClusterInfo> GetClusterInfoAsync();
    }
}