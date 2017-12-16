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
    public class BucketRamCheck : ICheck
    {
        IClusterService _clusterService;

        private readonly BucketRamCheckSettings _settings;
        public string ShortName { get; } = "Couchbase Document Check";

        public BucketRamCheck(BucketRamCheckSettings settings)
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
                string message = $"RAM useage threshold ({_settings.RamUsageThreshold}) breached on the following buckets: {string.Join(" | ", bucketWarnings)}";
                return new CheckResult(ShortName, NotificationLevel.Warning, message);
            }
            else
            {
                string okayMsg = "Bucket RAM usage within expected range";
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
                ulong memUsed = bucketConfig.BasicStats.MemUsed;
                ulong ramQuota = bucketConfig.Quota.Ram;
                double percentRamUsed = (memUsed / (double)ramQuota) * 100;

                if (percentRamUsed > _settings.RamUsageThreshold)
                {
                    yield return $"Bucket [{bucketConfig.Name}], RAM Usage [{percentRamUsed}%]";
                }
                else
                {
                    yield break;
                }
            }
        }
    }
}
