using Couchbase.Configuration.Server.Serialization;
using Couchmon.Couchbase;
using Nimator;
using System.Collections.Generic;
using System.Linq;

namespace Couchmon.Checks
{
    public class RamThresholdCheck : BucketCheck, ICheck
    {
        private readonly int _threshold;

        public RamThresholdCheck(IClusterService clusterService, int threshold) : base (clusterService)
        {
            ShortName = "Couchbase Bucket Document Check";
            _threshold = threshold;
        }

        protected override CheckResult ConvertToCheckResult(IEnumerable<string> bucketWarnings)
        {
            if (bucketWarnings.Any())
            {
                int warningCount = bucketWarnings.Count();
                string message = $"RAM usage threshold ({_threshold}) breached on the following buckets: {string.Join(" | ", bucketWarnings)}";
                return new CheckResult(ShortName, NotificationLevel.Warning, message);
            }
            else
            {
                string okayMsg = "Bucket RAM usage within expected range";
                return new CheckResult(ShortName, NotificationLevel.Okay, okayMsg);
            }
        }

        protected override IEnumerable<string> GetBucketsToWarnOn(IList<IBucketConfig> bucketConfigs)
        {
            if (!bucketConfigs.Any())
            {
                yield break;
            }

            foreach (var bucketConfig in bucketConfigs)
            {
                ulong memUsed = bucketConfig.BasicStats.MemUsed;
                ulong ramQuota = bucketConfig.Quota.Ram;
                double percentRamUsed = (memUsed / (double)ramQuota) * 100;

                if (percentRamUsed > _threshold)
                {
                    yield return $"Bucket [{bucketConfig.Name}], RAM Usage [{percentRamUsed}%]";
                }
            }
        }
    }
}
