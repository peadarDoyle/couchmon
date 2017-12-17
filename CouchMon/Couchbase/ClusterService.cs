using Couchbase;
using Couchbase.Authentication;
using Couchbase.Core;
using Couchbase.Management;
using System.Threading.Tasks;

namespace Couchmon.Couchbase
{
    public class ClusterService : IClusterService
    {
        private IClusterManager _clusterManager;

        public ClusterService(ICluster cluster, IAuthenticator authenticator)
        {
            cluster.Authenticate(authenticator);
            _clusterManager = cluster.CreateManager();
        }

        public async Task<IClusterInfo> GetClusterInfoAsync()
        {
            IResult<IClusterInfo> result = await _clusterManager.ClusterInfoAsync().ConfigureAwait(false);

            if (result.Success != true)
            {
                throw new CannotAccesClusterInfoException(result.Message, result.Exception);
            }

            return result.Value;
        }
    }
}
