using System;

namespace Couchmon.Couchbase
{
    public class CannotAccesClusterInfoException : Exception
    {
        public CannotAccesClusterInfoException(string message, Exception innerException) : base (message, innerException) { }
    }
}
