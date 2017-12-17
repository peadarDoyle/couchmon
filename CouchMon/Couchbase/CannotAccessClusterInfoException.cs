using System;

namespace Couchmon.Couchbase
{
    public class CouchMonNotInitializedException : Exception
    {
        public CouchMonNotInitializedException(string message, Exception innerException) : base (message, innerException) { }
    }
}
