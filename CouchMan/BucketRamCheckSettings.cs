using Nimator;

namespace CouchMan
{
    public class BucketRamCheckSettings : ICheckSettings
    {
        public int RamUsageThreshold { get; set; }

        public ICheck ToCheck()
        {
            var clusterService = CouchMonContext.GetInstance<IClusterService>();
            return new BucketRamCheck(clusterService, RamUsageThreshold);
        }
    }
}