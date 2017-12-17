using Couchmon.Couchbase;
using Nimator;

namespace Couchmon.Checks
{
    public class BucketRamCheckSettings : ICheckSettings
    {
        public int RamUsageThreshold { get; set; }

        public ICheck ToCheck()
        {
            var clusterService = CouchmonContext.GetInstance<IClusterService>();
            return new BucketRamCheck(clusterService, RamUsageThreshold);
        }
    }
}