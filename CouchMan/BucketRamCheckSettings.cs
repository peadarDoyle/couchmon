using Nimator;

namespace CouchMan
{
    public class BucketRamCheckSettings : ICheckSettings
    {
        public int RamUsageThreshold { get; set; }

        public ICheck ToCheck()
        {
            return new BucketRamCheck(this);
        }
    }
}