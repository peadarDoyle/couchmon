using Couchbase.Configuration.Server.Serialization;
using Couchbase.Core;
using Couchmon.Couchbase;
using Nimator;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Couchmon.Checks
{
    public class BucketDocumentCheck : ICheck
    {
        public string ShortName { get; } = "Couchbase Ram Check";

        private readonly IClusterService _clusterService;
        private readonly int _documentThreshold;

        public BucketDocumentCheck(IClusterService clusterService, int threshold)
        {
            _clusterService = clusterService;
            _documentThreshold = threshold;
        }

        public async Task<ICheckResult> RunAsync()
        {
            IClusterInfo clusterInfo;

            try
            {
                clusterInfo = await _clusterService.GetClusterInfoAsync();
            }
            catch (CouchMonNotInitializedException ex)
            {
                return new CheckResult(ShortName, NotificationLevel.Critical, ex.Message);
            }

            IEnumerable<string> bucketWarnings = GetBucketsToWarnOn(clusterInfo.BucketConfigs());

            if (bucketWarnings.Any())
            {
                int warningCount = bucketWarnings.Count();
                string msg = $"Document limit ({_documentThreshold}) breached on the following buckets: {string.Join(" | ", bucketWarnings)}";
                return new CheckResult(ShortName, NotificationLevel.Warning, msg);
            }
            else
            {
                string okayMsg = "Bucket document count within expected range";
                return new CheckResult(ShortName, NotificationLevel.Okay, okayMsg);
            }
        }

        private IEnumerable<string> GetBucketsToWarnOn(IList<IBucketConfig> bucketConfigs)
        {
            if (!bucketConfigs.Any())
            {
                yield break;
            }

            foreach (var bucketConfig in bucketConfigs)
            {
                long documentCount = bucketConfig.BasicStats.ItemCount;

                if (documentCount > _documentThreshold)
                {
                    yield return $"Bucket [{bucketConfig.Name}], Document Count [{documentCount}]";
                }
            }
        }
    }
}
