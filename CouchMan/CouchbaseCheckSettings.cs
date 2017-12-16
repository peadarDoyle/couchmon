using Nimator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CouchMan
{
    public class CouchbaseCheckSettings : ICheckSettings
    {
        public int DocumentLimit { get; } = 100000;

        public ICheck ToCheck()
        {
            return new BucketDocumentCountCheck();

        }
    }
}
