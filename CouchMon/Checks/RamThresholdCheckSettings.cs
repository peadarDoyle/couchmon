using Couchmon.Couchbase;
using Nimator;

namespace Couchmon.Checks
{
    public class RamThresholdCheckSettings : ICheckSettings
    {
        public int RamUsageThreshold { get; set; }

        public ICheck ToCheck()
        {
            var clusterService = CouchmonContext.GetInstance<IClusterService>();
            return new RamThresholdCheck(clusterService, RamUsageThreshold);
        }
    }
}