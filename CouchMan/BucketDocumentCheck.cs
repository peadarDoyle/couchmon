using Couchbase.Authentication;
using Couchbase.Configuration.Server.Serialization;
using Couchbase.Core;
using Nimator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CouchMan
{
    public class BucketDocumentCheck : ICheck
    {
        IClusterService _clusterService;

        private readonly BucketDocumentCheckSettings _settings;
        public string ShortName { get; } = "Couchbase Ram Check";

        public BucketDocumentCheck(BucketDocumentCheckSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            IAuthenticator authenticator = new PasswordAuthenticator("Administrator", "badpassword");
            string[] urls = new[] { "http://localhost/8091" };
            _clusterService = new ClusterService(urls, authenticator);
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
                string msg = $"Document limit ({_settings.DocumentThreshold}) breached on the following buckets: {string.Join(" | ", bucketWarnings)}";
                return new CheckResult(ShortName, NotificationLevel.Warning, msg);
            }
            else
            {
                string okayMsg = "Bucket document count within expected range";
                return new CheckResult(ShortName, NotificationLevel.Okay, okayMsg);
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
                long documentCount = bucketConfig.BasicStats.ItemCount;

                if (documentCount > _settings.DocumentThreshold)
                {
                    yield return $"Bucket [{bucketConfig.Name}], Document Count [{documentCount}]";
                }
            }
        }
    }
}
