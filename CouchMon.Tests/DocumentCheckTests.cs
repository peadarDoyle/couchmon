using Couchbase.Configuration.Server.Serialization;
using Couchbase.Core;
using Couchmon.Checks;
using Couchmon.Couchbase;
using Moq;
using Nimator;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Couchmon.Tests
{
    [TestFixture]
    public class DocumentCheckTests
    {
        private DocumentThresholdCheck _target;

        private Mock<IClusterService> _clusterService;
        private Mock<IClusterInfo> _clusterInfo;
        private Mock<IBucketConfig> _bucketConfig1;
        private Mock<IBucketConfig> _bucketConfig2;

        private int _threshold = 3;
        private int _belowThreshold = 2;
        private int _aboveThreshold = 4;

        [SetUp]
        public void SetUp()
        {
            _clusterService = new Mock<IClusterService>();
            _clusterInfo = new Mock<IClusterInfo>();
            _bucketConfig1 = new Mock<IBucketConfig>();
            _bucketConfig2 = new Mock<IBucketConfig>();

            _clusterService.Setup(x => x.GetClusterInfoAsync())
                           .ReturnsAsync(_clusterInfo.Object);

            _clusterService.Setup(x => x.GetClusterInfoAsync())
                           .ReturnsAsync(_clusterInfo.Object);

            _clusterInfo.Setup(x => x.BucketConfigs())
                        .Returns(new List<IBucketConfig> { _bucketConfig1.Object, _bucketConfig2.Object });

            _bucketConfig1.SetupGet(x => x.BasicStats)
                          .Returns(new BasicStats { ItemCount = _belowThreshold });

            _target = new DocumentThresholdCheck(_clusterService.Object, _threshold);
        }

        [TestCase(NotificationLevel.Okay)]
        public async Task RunAsync_AllBucketsWithDocumentCountWithinThreshold_OkayNotification(NotificationLevel expectedLevel)
        {
            _bucketConfig2.SetupGet(x => x.BasicStats).Returns(new BasicStats { ItemCount = _belowThreshold });

            ICheckResult result = await _target.RunAsync();

            Assert.AreEqual(expectedLevel, result.Level);
            Assert.IsTrue(result.RenderPlainText().Contains("Bucket document count within expected range"));
            _clusterService.Verify(x => x.GetClusterInfoAsync(), Times.Once);
        }

        [TestCase(NotificationLevel.Warning)]
        public async Task RunAsync_SomeBucketsWithDocumentCountAboveThreshold_WarningNotification(NotificationLevel expectedLevel)
        {
            _bucketConfig2.SetupGet(x => x.BasicStats).Returns(new BasicStats { ItemCount = _aboveThreshold });

            ICheckResult result = await _target.RunAsync();

            Assert.AreEqual(expectedLevel, result.Level);
            _clusterService.Verify(x => x.GetClusterInfoAsync(), Times.Once);
        }

        [TestCase(NotificationLevel.Critical)]
        public async Task RunAsync_ClusterApiCannotBeAccessed_CriticalNotification(NotificationLevel expectedLevel)
        {
            var msg = "Cannot Access Cluster";
            var ex = new CannotAccesClusterInfoException(msg, new Exception());
            _clusterService.Setup(x => x.GetClusterInfoAsync()).ThrowsAsync(ex);

            ICheckResult result = await _target.RunAsync();

            Assert.AreEqual(expectedLevel, result.Level);
            Assert.IsTrue(result.RenderPlainText().Contains(msg));
            _clusterService.Verify(x => x.GetClusterInfoAsync(), Times.Once);
        }
    }
}