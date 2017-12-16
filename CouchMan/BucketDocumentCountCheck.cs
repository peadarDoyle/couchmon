using Couchbase.Authentication;
using Couchbase.Configuration.Server.Serialization;
using Couchbase.Core;
using Nimator;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CouchMan
{
    public class BucketDocumentCountCheck : ICheck
    {
        IClusterService _clusterService;

        private readonly CouchbaseCheckSettings settings;
        public string ShortName { get; } = "Couchbase - Document Limit Check";

        public BucketDocumentCountCheck()
        {
            IAuthenticator authenticator = new PasswordAuthenticator("Administrator", "badpassword");
            string[] urls = new[] { "http://localhost/8091" };
            _clusterService = new ClusterService(urls, authenticator);

            settings = new CouchbaseCheckSettings();
        }

        public async Task<ICheckResult> RunAsync()
        {
            IClusterInfo clusterInfo;

            try
            {
                clusterInfo = await _clusterService.GetClusterInfoAsync();
            }
            catch (CannotAccessClusterInfoException ex)
            {
                return new CheckResult(ShortName, NotificationLevel.Critical, ex.Message);
            }

            IEnumerable<string> bucketWarnings = GetBucketsToWarnOn(clusterInfo.BucketConfigs());

            if (bucketWarnings.Any())
            {
                int warningCount = bucketWarnings.Count();
                string message = $"Document limit ({Settings.DocumentLimit}) breached on the following buckets: {string.Join(" | ", bucketWarnings)}";
                return new CheckResult(ShortName, NotificationLevel.Warning, message);
            }
            else
            {
                return new CheckResult(ShortName, NotificationLevel.Okay);
            }
        }

        public IEnumerable<string> GetBucketsToWarnOn(IList<IBucketConfig> bucketConfigs)
        {
            if (!bucketConfigs.Any())
            {
                yield break;
            }

            foreach (var bucketConfig in bucketConfigs)
            {
                long itemCount = bucketConfig.BasicStats.ItemCount;

                if (itemCount > Settings.DocumentLimit)
                {
                    yield return $"Bucket [{bucketConfig.Name}], Item Count [{itemCount}]";
                }
                else
                {
                    yield break;
                }
            }
        }
    }
}
