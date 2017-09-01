using System;
using System.Linq;
using NUnit.Framework;

namespace SJP.DiskCache.Tests
{
    [TestFixture]
    public class MfuCachePolicyTests
    {
        [Test]
        public void GetExpiredEntries_GivenNullEntries_ThrowsArgNullException()
        {
            var policy = new MfuCachePolicy<string>();
            Assert.Throws<ArgumentNullException>(() => policy.GetExpiredEntries(null, 1));
        }

        [Test]
        public void GetExpiredEntries_GivenZeroSize_ThrowsArgOutOfRangeException()
        {
            var policy = new MfuCachePolicy<string>();
            Assert.Throws<ArgumentOutOfRangeException>(() => policy.GetExpiredEntries(Enumerable.Empty<ICacheEntry<string>>(), 0));
        }
    }
}
