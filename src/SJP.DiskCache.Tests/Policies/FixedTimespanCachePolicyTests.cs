using System;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace SJP.DiskCache.Tests
{
    [TestFixture]
    public class FixedTimespanCachePolicyTests
    {
        [Test]
        public void Ctor_GivenNegativeTimeSpan_ThrowsArgOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new FixedTimespanCachePolicy<string>(TimeSpan.FromMilliseconds(-1)));
        }

        [Test]
        public void Ctor_GivenZeroTimeSpan_ThrowsArgOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new FixedTimespanCachePolicy<string>(TimeSpan.FromMilliseconds(0)));
        }

        [Test]
        public void GetExpiredEntries_GivenNullEntries_ThrowsArgNullException()
        {
            var policy = new FixedTimespanCachePolicy<string>(TimeSpan.FromMilliseconds(1));
            Assert.Throws<ArgumentNullException>(() => policy.GetExpiredEntries(null, 1));
        }

        [Test]
        public void GetExpiredEntries_GivenZeroSize_ThrowsArgOutOfRangeException()
        {
            var policy = new FixedTimespanCachePolicy<string>(TimeSpan.FromMilliseconds(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => policy.GetExpiredEntries(Enumerable.Empty<ICacheEntry<string>>(), 0));
        }

        [Test]
        public void GetExpiredEntries_WhenCacheNotExhausted_ReturnsEmptySet()
        {
            var policy = new FixedTimespanCachePolicy<string>(TimeSpan.FromSeconds(100));

            var cacheEntry = new Mock<ICacheEntry<string>>();
            cacheEntry.Setup(k => k.CreationTime).Returns(DateTime.Now);
            cacheEntry.Setup(k => k.Size).Returns(1);
            cacheEntry.Setup(k => k.Key).Returns("test");

            var expired = policy.GetExpiredEntries(new[] { cacheEntry.Object }, 100).ToList();

            Assert.AreEqual(0, expired.Count);
        }

        [Test]
        public void GetExpiredEntries_WhenOnlyValueExceedsTimePeriod_ReturnsValue()
        {
            var policy = new FixedTimespanCachePolicy<string>(TimeSpan.FromMilliseconds(1));

            var cacheEntry = new Mock<ICacheEntry<string>>();
            cacheEntry.Setup(k => k.CreationTime).Returns(DateTime.Now - TimeSpan.FromSeconds(1));
            cacheEntry.Setup(k => k.Size).Returns(1);
            cacheEntry.Setup(k => k.Key).Returns("test");

            var expired = policy.GetExpiredEntries(new[] { cacheEntry.Object }, 100).ToList();

            Assert.AreEqual(1, expired.Count);
        }

        [Test]
        public void GetExpiredEntries_WhenCacheTimesExpired_ReturnsNonEmptyCollection()
        {
            var policy = new FixedTimespanCachePolicy<string>(TimeSpan.FromSeconds(30));

            var cacheEntry1 = new Mock<ICacheEntry<string>>();
            cacheEntry1.Setup(k => k.CreationTime).Returns(DateTime.Now - TimeSpan.FromMinutes(2));
            cacheEntry1.Setup(k => k.Size).Returns(5);
            cacheEntry1.Setup(k => k.Key).Returns("test1");

            var cacheEntry2 = new Mock<ICacheEntry<string>>();
            cacheEntry2.Setup(k => k.CreationTime).Returns(DateTime.Now);
            cacheEntry2.Setup(k => k.Size).Returns(5);
            cacheEntry2.Setup(k => k.Key).Returns("test2");

            var cacheEntry3 = new Mock<ICacheEntry<string>>();
            cacheEntry3.Setup(k => k.CreationTime).Returns(DateTime.Now - TimeSpan.FromMinutes(2));
            cacheEntry3.Setup(k => k.Size).Returns(5);
            cacheEntry3.Setup(k => k.Key).Returns("test3");

            var expired = policy.GetExpiredEntries(new[] { cacheEntry1.Object, cacheEntry2.Object, cacheEntry3.Object }, 20).ToList();

            Assert.AreEqual(2, expired.Count);
        }

        [Test]
        public void GetExpiredEntries_WhenCacheTimesExpired_ReturnsEarliestValue()
        {
            var policy = new FixedTimespanCachePolicy<string>(TimeSpan.FromSeconds(30));

            var cacheEntry1 = new Mock<ICacheEntry<string>>();
            cacheEntry1.Setup(k => k.CreationTime).Returns(DateTime.Now - TimeSpan.FromMinutes(2));
            cacheEntry1.Setup(k => k.Size).Returns(5);
            cacheEntry1.Setup(k => k.Key).Returns("test1");

            var cacheEntry2 = new Mock<ICacheEntry<string>>();
            cacheEntry2.Setup(k => k.CreationTime).Returns(DateTime.Now);
            cacheEntry2.Setup(k => k.Size).Returns(5);
            cacheEntry2.Setup(k => k.Key).Returns("test2");

            var cacheEntry3 = new Mock<ICacheEntry<string>>();
            cacheEntry3.Setup(k => k.CreationTime).Returns(DateTime.Now - TimeSpan.FromMinutes(2));
            cacheEntry3.Setup(k => k.Size).Returns(5);
            cacheEntry3.Setup(k => k.Key).Returns("test3");

            var expired = policy.GetExpiredEntries(new[] { cacheEntry1.Object, cacheEntry2.Object, cacheEntry3.Object }, 20).ToList();

            Assert.AreEqual(2, expired.Count);
            var firstExpiredKey = expired[0].Key;
            var lastExpiredKey = expired.Last().Key;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(cacheEntry1.Object.Key, firstExpiredKey);
                Assert.AreEqual(cacheEntry3.Object.Key, lastExpiredKey);
            });
        }
    }
}
