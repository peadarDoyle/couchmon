using Couchbase;
using Couchbase.Authentication;
using Couchbase.Core;
using Couchbase.Management;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System.Threading.Tasks;

namespace CouchMan.Tests
{
    [TestFixture]
    public class ClusterServiceTest
    {
        private IClusterService _target;

        private Mock<ICluster> _cluster;
        private Mock<IAuthenticator> _authenticator;
        private Mock<IClusterManager> _clusterManager;
        private Mock<IResult<IClusterInfo>> _clusterInfoResult;
        private Mock<IClusterInfo> _clusterInfo;

        [SetUp]
        public void SetUp()
        {
            var autoMocker = new AutoMocker();

            _cluster = autoMocker.GetMock<ICluster>();
            _authenticator = autoMocker.GetMock<IAuthenticator>();
            _clusterManager = autoMocker.GetMock<IClusterManager>();
            _clusterInfoResult = autoMocker.GetMock<IResult<IClusterInfo>>();
            _clusterInfo = autoMocker.GetMock<IClusterInfo>();

            _cluster.Setup(x => x.CreateManager())
                    .Returns(_clusterManager.Object);

            _target = autoMocker.CreateInstance<ClusterService>();
        }

        [Test]
        public async Task GetClusterInfo_TryGegClusterInfo_SuccefullyGetClusterInfo()
        {
            _clusterManager.Setup(x => x.ClusterInfoAsync()).ReturnsAsync(_clusterInfoResult.Object);
            _clusterInfoResult.SetupGet(x => x.Success).Returns(true);
            _clusterInfoResult.SetupGet(x => x.Value).Returns(_clusterInfo.Object);

            IClusterInfo info = await _target.GetClusterInfoAsync();

            Assert.AreEqual(_clusterInfo.Object, info);
            _clusterManager.Verify(x => x.ClusterInfoAsync(), Times.Once);
        }

        [Test]
        public void GetClusterInfo_TryGegClusterInfo_ThrowsCannotAccessClusterInfoException()
        {
            string message = "an exception message";

            _clusterManager.Setup(x => x.ClusterInfoAsync()).ReturnsAsync(_clusterInfoResult.Object);
            _clusterInfoResult.SetupGet(x => x.Success).Returns(false);
            _clusterInfoResult.SetupGet(x => x.Message).Returns(message);

            var exception = Assert.ThrowsAsync<CannotAccessClusterInfoException>(async () => await _target.GetClusterInfoAsync());

            Assert.AreEqual(exception.Message, message);
            _clusterManager.Verify(x => x.ClusterInfoAsync(), Times.Once);
        }
    }
}