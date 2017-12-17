using Couchbase.Configuration.Server.Serialization;
using Couchmon.Couchbase;
using Nimator;
using System.Collections.Generic;
using System.Linq;

namespace Couchmon.Checks
{
    /// <summary>
    /// Check that Couchbase bucket Document count does not go over a healthy threshold.
    /// </summary>
    public class DocumentThresholdCheck : BucketCheck
    {
        private readonly int _threshold;

        public DocumentThresholdCheck(IClusterService clusterService, int threshold) : base (clusterService)
        {
            ShortName = "Couchbase Bucket Document Check";
            _threshold = threshold;
        }

        protected override IEnumerable<string> GetBucketsToWarnOn(IList<IBucketConfig> bucketConfigs)
        {
            if (!bucketConfigs.Any())
            {
                yield break;
            }

            foreach (var bucketConfig in bucketConfigs)
            {
                long documentCount = bucketConfig.BasicStats.ItemCount;

                if (documentCount > _threshold)
                {
                    yield return $"Bucket [{bucketConfig.Name}], Document Count [{documentCount}]";
                }
            }
        }

        protected override CheckResult ConvertToCheckResult(IEnumerable<string> bucketWarnings)
        {
            if (bucketWarnings.Any())
            {
                int warningCount = bucketWarnings.Count();
                string msg = $"Document limit ({_threshold}) breached on the following buckets: {string.Join(" | ", bucketWarnings)}";
                return new CheckResult(ShortName, NotificationLevel.Warning, msg);
            }
            else
            {
                string okayMsg = "Bucket document count within expected range";
                return new CheckResult(ShortName, NotificationLevel.Okay, okayMsg);
            }
        }
    }
}
