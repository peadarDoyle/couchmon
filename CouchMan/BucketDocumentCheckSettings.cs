using Nimator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CouchMan
{
    public class BucketDocumentCheckSettings : ICheckSettings
    {
        public int DocumentThreshold { get; set; }

        public ICheck ToCheck()
        {
            return new BucketDocumentCheck(this);

        }
    }
}
