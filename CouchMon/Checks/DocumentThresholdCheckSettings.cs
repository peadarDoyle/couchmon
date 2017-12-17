using Couchmon.Couchbase;
using Nimator;

namespace Couchmon.Checks
{
    public class DocumentThresholdCheckSettings : ICheckSettings
    {
        public int DocumentThreshold { get; set; }

        public ICheck ToCheck()
        {
            var clusterService = CouchmonContext.GetInstance<IClusterService>();
            return new DocumentThresholdCheck(clusterService, DocumentThreshold);
        }
    }
}
