using Couchmon.Couchbase;
using Nimator;

namespace Couchmon.Checks
{
    public class BucketDocumentCheckSettings : ICheckSettings
    {
        public int DocumentThreshold { get; set; }

        public ICheck ToCheck()
        {
            var clusterService = CouchmonContext.GetInstance<IClusterService>();
            return new BucketDocumentCheck(clusterService, DocumentThreshold);
        }
    }
}
