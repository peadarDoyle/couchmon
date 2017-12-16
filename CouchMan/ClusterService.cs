using Couchbase;
using Couchbase.Authentication;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using Couchbase.Management;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CouchMan
{
    public class ClusterService : IClusterService
    {
        private ClientConfiguration _clientConfig;
        private IAuthenticator _authenticator;

        public ClusterService(string[] urls, IAuthenticator authenticator)
        {
            _clientConfig = new ClientConfiguration
            {
                Servers = urls.Select(url => new Uri(url)).ToList()
            };

            _authenticator = authenticator;
        }

        public async Task<IClusterInfo> GetClusterInfoAsync()
        {
            IResult<IClusterInfo> result;

            using (var cluster = new Cluster(_clientConfig))
            {
                cluster.Authenticate(_authenticator);
                IClusterManager manager = cluster.CreateManager();
                result = await manager.ClusterInfoAsync().ConfigureAwait(false);
            }

            if (result.Success != true)
            {
                throw new CannotAccessClusterInfoException(result.Message, result.Exception);
            }

            return result.Value;
        }
    }
}
