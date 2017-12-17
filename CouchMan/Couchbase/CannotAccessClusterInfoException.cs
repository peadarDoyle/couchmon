using System;

namespace CouchMan
{
    public class CannotAccessClusterInfoException : Exception
    {
        public CannotAccessClusterInfoException(string message, Exception innerException) : base (message, innerException) { }
    }
}
