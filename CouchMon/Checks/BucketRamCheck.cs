using Couchbase.Configuration.Server.Serialization;
using Couchbase.Core;
using Couchmon.Couchbase;
using Nimator;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Couchmon.Checks
{
    public class BucketRamCheck : ICheck
    {
        public string ShortName { get; } = "Couchbase Document Check";

        private readonly IClusterService _clusterService;
        private readonly int _ramUsageThreshold;

        public BucketRamCheck(IClusterService clusterService, int threshold)
        {
            _clusterService = clusterService;
            _ramUsageThreshold = threshold;
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
                string message = $"RAM useage threshold ({_ramUsageThreshold}) breached on the following buckets: {string.Join(" | ", bucketWarnings)}";
                return new CheckResult(ShortName, NotificationLevel.Warning, message);
            }
            else
            {
                string okayMsg = "Bucket RAM usage within expected range";
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
                ulong memUsed = bucketConfig.BasicStats.MemUsed;
                ulong ramQuota = bucketConfig.Quota.Ram;
                double percentRamUsed = (memUsed / (double)ramQuota) * 100;

                if (percentRamUsed > _ramUsageThreshold)
                {
                    yield return $"Bucket [{bucketConfig.Name}], RAM Usage [{percentRamUsed}%]";
                }
            }
        }
    }
}
