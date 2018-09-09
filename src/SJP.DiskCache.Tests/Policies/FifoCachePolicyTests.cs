using System;
using System.Linq;
using NUnit.Framework;
using Moq;

namespace SJP.DiskCache.Tests
{
    [TestFixture]
    internal static class FifoCachePolicyTests
    {
        [Test]
        public static void GetExpiredEntries_GivenNullEntries_ThrowsArgNullException()
        {
            var policy = new FifoCachePolicy<string>();
            Assert.Throws<ArgumentNullException>(() => policy.GetExpiredEntries(null, 1));
        }

        [Test]
        public static void GetExpiredEntries_GivenZeroSize_ThrowsArgOutOfRangeException()
        {
            var policy = new FifoCachePolicy<string>();
            Assert.Throws<ArgumentOutOfRangeException>(() => policy.GetExpiredEntries(Enumerable.Empty<ICacheEntry<string>>(), 0));
        }

        [Test]
        public static void GetExpiredEntries_WhenCacheNotExhausted_ReturnsEmptySet()
        {
            var policy = new FifoCachePolicy<string>();

            var cacheEntry = new Mock<ICacheEntry<string>>();
            cacheEntry.Setup(k => k.CreationTime).Returns(DateTime.Now);
            cacheEntry.Setup(k => k.Size).Returns(1);
            cacheEntry.Setup(k => k.Key).Returns("test");

            var expired = policy.GetExpiredEntries(new[] { cacheEntry.Object }, 100).ToList();

            Assert.AreEqual(0, expired.Count);
        }

        [Test]
        public static void GetExpiredEntries_WhenOnlyValueExceedsSize_ReturnsValue()
        {
            var policy = new FifoCachePolicy<string>();

            var cacheEntry = new Mock<ICacheEntry<string>>();
            cacheEntry.Setup(k => k.CreationTime).Returns(DateTime.Now);
            cacheEntry.Setup(k => k.Size).Returns(101);
            cacheEntry.Setup(k => k.Key).Returns("test");

            var expired = policy.GetExpiredEntries(new[] { cacheEntry.Object }, 100).ToList();

            Assert.AreEqual(1, expired.Count);
        }

        [Test]
        public static void GetExpiredEntries_WhenCacheExhausted_ReturnsNonEmptyCollection()
        {
            var policy = new FifoCachePolicy<string>();

            var cacheEntry1 = new Mock<ICacheEntry<string>>();
            cacheEntry1.Setup(k => k.CreationTime).Returns(new DateTime(2017, 1, 1));
            cacheEntry1.Setup(k => k.Size).Returns(5);
            cacheEntry1.Setup(k => k.Key).Returns("test1");

            var cacheEntry2 = new Mock<ICacheEntry<string>>();
            cacheEntry2.Setup(k => k.CreationTime).Returns(new DateTime(2017, 1, 2));
            cacheEntry2.Setup(k => k.Size).Returns(5);
            cacheEntry2.Setup(k => k.Key).Returns("test2");

            var cacheEntry3 = new Mock<ICacheEntry<string>>();
            cacheEntry3.Setup(k => k.CreationTime).Returns(new DateTime(2017, 1, 3));
            cacheEntry3.Setup(k => k.Size).Returns(5);
            cacheEntry3.Setup(k => k.Key).Returns("test3");

            var expired = policy.GetExpiredEntries(new[] { cacheEntry1.Object, cacheEntry2.Object, cacheEntry3.Object }, 12).ToList();

            Assert.AreEqual(1, expired.Count);
        }

        [Test]
        public static void GetExpiredEntries_WhenCacheExhausted_ReturnsEarliestValue()
        {
            var policy = new FifoCachePolicy<string>();

            var cacheEntry1 = new Mock<ICacheEntry<string>>();
            cacheEntry1.Setup(k => k.CreationTime).Returns(new DateTime(2017, 1, 1));
            cacheEntry1.Setup(k => k.Size).Returns(5);
            cacheEntry1.Setup(k => k.Key).Returns("test1");

            var cacheEntry2 = new Mock<ICacheEntry<string>>();
            cacheEntry2.Setup(k => k.CreationTime).Returns(new DateTime(2017, 1, 2));
            cacheEntry2.Setup(k => k.Size).Returns(5);
            cacheEntry2.Setup(k => k.Key).Returns("test2");

            var cacheEntry3 = new Mock<ICacheEntry<string>>();
            cacheEntry3.Setup(k => k.CreationTime).Returns(new DateTime(2017, 1, 3));
            cacheEntry3.Setup(k => k.Size).Returns(5);
            cacheEntry3.Setup(k => k.Key).Returns("test3");

            var expired = policy.GetExpiredEntries(new[] { cacheEntry1.Object, cacheEntry2.Object, cacheEntry3.Object }, 12).ToList();
            var expiredKey = expired.Single().Key;

            Assert.AreEqual(cacheEntry1.Object.Key, expiredKey);
        }
    }
}
