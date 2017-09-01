using System;
using System.Linq;
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
    }
}
