using Nimator;

namespace CouchMan
{
    public class BucketDocumentCheckSettings : ICheckSettings
    {
        public int DocumentThreshold { get; set; }

        public ICheck ToCheck()
        {
            var clusterService = CouchbaseContext.GetInstance<IClusterService>();
            return new BucketDocumentCheck(clusterService, DocumentThreshold);
        }
    }
}
