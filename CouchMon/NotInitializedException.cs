using System;

namespace Couchmon.Couchbase
{
    public class NotInitializedException : Exception
    {
        private const string message = "You must initialize CouchMon before creating the INimator object";

        public NotInitializedException() : base (message) { }
    }
}
