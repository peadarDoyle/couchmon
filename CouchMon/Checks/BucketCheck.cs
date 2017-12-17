using Couchbase.Configuration.Server.Serialization;
using Couchbase.Core;
using Couchmon.Couchbase;
using Nimator;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Couchmon.Checks
{
    public abstract class BucketCheck : ICheck
    {
        public string ShortName { get; protected set; }
        protected readonly IClusterService _clusterService;

        public BucketCheck(IClusterService clusterService)
        {
            _clusterService = clusterService;
        }

        public async Task<ICheckResult> RunAsync()
        {
            IClusterInfo clusterInfo;

            try
            {
                clusterInfo = await _clusterService.GetClusterInfoAsync();
            }
            catch (CannotAccesClusterInfoException ex)
            {
                return new CheckResult(ShortName, NotificationLevel.Critical, ex.Message);
            }

            IEnumerable<string> bucketWarnings = GetBucketsToWarnOn(clusterInfo.BucketConfigs());
            return ConvertToCheckResult(bucketWarnings);
        }

        protected abstract IEnumerable<string> GetBucketsToWarnOn(IList<IBucketConfig> bucketConfigs);
        protected abstract CheckResult ConvertToCheckResult(IEnumerable<string> bucketsToWarnOn);
    }
}
