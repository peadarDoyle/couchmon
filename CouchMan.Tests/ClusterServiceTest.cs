using NUnit.Framework;
using Couchbase.Authentication;
using Couchbase.Core;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace CouchMan.Tests
{
    [TestFixture]
    public class ClusterServiceTest
    {
        private IClusterService _target;

        [SetUp]
        public void SetUp()
        {
            IAuthenticator authenticator = new PasswordAuthenticator("Administrator", "badpassword");
            string[] urls = new[] { "http://localhost/8091" };
            _target = new ClusterService(urls, authenticator);
        }

        [Test]
        public async Task GetClusterInfo_TryGegClusterInfo_SuccefullyGetClusterInfo()
        {
            IClusterInfo info = await _target.GetClusterInfoAsync();
            Assert.IsTrue(false);
        }
    }
}