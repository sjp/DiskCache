using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using System.IO;
using System.Collections.Generic;

namespace SJP.DiskCache.Tests
{
    [TestFixture]
    public class DiskCacheTests
    {
        [Test]
        public void Ctor_GivenNullDirectory_ThrowsArgNullException()
        {
            var cachePolicy = Mock.Of<ICachePolicy>();
            Assert.Throws<ArgumentNullException>(() => new DiskCache((DirectoryInfo)null, cachePolicy, 123));
        }

        [Test]
        public void Ctor_GivenNullCachePolicy_ThrowsArgNullException()
        {
            var dir = new DirectoryInfo(Environment.CurrentDirectory);
            Assert.Throws<ArgumentNullException>(() => new DiskCache(dir, null, 123));
        }

        [Test]
        public void Ctor_GivenZeroStorageCapacity_ThrowsArgOutOfRangeException()
        {
            var dir = new DirectoryInfo(Environment.CurrentDirectory);
            var cachePolicy = Mock.Of<ICachePolicy>();

            Assert.Throws<ArgumentOutOfRangeException>(() => new DiskCache(dir, cachePolicy, 0));
        }

        [Test]
        public void Ctor_GivenNegativeInterval_ThrowsArgOutOfRangeException()
        {
            var dir = new DirectoryInfo(Environment.CurrentDirectory);
            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;

            Assert.Throws<ArgumentOutOfRangeException>(() => new DiskCache(dir, cachePolicy, size, TimeSpan.FromSeconds(-1)));
        }

        [Test]
        public void Ctor_GivenZeroInterval_ThrowsArgOutOfRangeException()
        {
            var dir = new DirectoryInfo(Environment.CurrentDirectory);
            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;

            Assert.Throws<ArgumentOutOfRangeException>(() => new DiskCache(dir, cachePolicy, size, TimeSpan.FromSeconds(0)));
        }

        [Test]
        public void Ctor_GivenNullDirectoryPath_ThrowsArgNullException()
        {
            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;

            Assert.Throws<ArgumentNullException>(() => new DiskCache((string)null, cachePolicy, size));
        }

        [Test]
        public void Ctor_GivenEmptyDirectoryPath_ThrowsArgNullException()
        {
            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;

            Assert.Throws<ArgumentNullException>(() => new DiskCache(string.Empty, cachePolicy, size));
        }

        [Test]
        public void Ctor_GivenWhiteSpaceDirectoryPath_ThrowsArgNullException()
        {
            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;

            Assert.Throws<ArgumentNullException>(() => new DiskCache("   ", cachePolicy, size));
        }

        [Test]
        public void Ctor_GivenNonExistentDirectory_ThrowsDirNotFoundException()
        {
            var dir = Path.Combine(Environment.CurrentDirectory, "asdasdasd");
            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;

            Assert.Throws<DirectoryNotFoundException>(() => new DiskCache(dir, cachePolicy, size));
        }

        [Test]
        public void ContainsKey_GivenNullKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "containskey_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;
            using (var cache = new DiskCache(testDir, cachePolicy, size))
            {
                Assert.Throws<ArgumentNullException>(() => cache.ContainsKey(null));
            }

            testDir.Delete(true);
        }

        [Test]
        public void ContainsKey_GivenEmptyKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "containskey_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;
            using (var cache = new DiskCache(testDir, cachePolicy, size))
            {
                Assert.Throws<ArgumentNullException>(() => cache.ContainsKey(string.Empty));
            }

            testDir.Delete(true);
        }

        [Test]
        public void ContainsKey_GivenWhiteSpaceKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "containskey_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;
            using (var cache = new DiskCache(testDir, cachePolicy, size))
            {
                Assert.Throws<ArgumentNullException>(() => cache.ContainsKey("  "));
            }

            testDir.Delete(true);
        }

        [Test]
        public void ContainsKeyAsync_GivenNullKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "containskeyasync_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;
            using (var cache = new DiskCache(testDir, cachePolicy, size))
            {
                Assert.ThrowsAsync<ArgumentNullException>(async () => await cache.ContainsKeyAsync(null).ConfigureAwait(false));
            }

            testDir.Delete(true);
        }

        [Test]
        public void ContainsKeyAsync_GivenEmptyKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "containskeyasync_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;
            using (var cache = new DiskCache(testDir, cachePolicy, size))
            {
                Assert.ThrowsAsync<ArgumentNullException>(async () => await cache.ContainsKeyAsync(string.Empty).ConfigureAwait(false));
            }

            testDir.Delete(true);
        }

        [Test]
        public void ContainsKeyAsync_GivenWhiteSpaceKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "containskeyasync_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;
            using (var cache = new DiskCache(testDir, cachePolicy, size))
            {
                Assert.ThrowsAsync<ArgumentNullException>(async () => await cache.ContainsKeyAsync("  ").ConfigureAwait(false));
            }

            testDir.Delete(true);
        }

        [Test]
        public void GetValue_GivenNullKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "getvalue_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;
            using (var cache = new DiskCache(testDir, cachePolicy, size))
            {
                Assert.Throws<ArgumentNullException>(() => cache.GetValue(null));
            }

            testDir.Delete(true);
        }

        [Test]
        public void GetValue_GivenEmptyKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "getvalue_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;
            using (var cache = new DiskCache(testDir, cachePolicy, size))
            {
                Assert.Throws<ArgumentNullException>(() => cache.GetValue(string.Empty));
            }

            testDir.Delete(true);
        }

        [Test]
        public void GetValue_GivenWhiteSpaceKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "getvalue_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;
            using (var cache = new DiskCache(testDir, cachePolicy, size))
            {
                Assert.Throws<ArgumentNullException>(() => cache.GetValue("  "));
            }

            testDir.Delete(true);
        }

        [Test]
        public void GetValueAsync_GivenNullKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "getvalueasync_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;
            using (var cache = new DiskCache(testDir, cachePolicy, size))
            {
                Assert.ThrowsAsync<ArgumentNullException>(async () => await cache.GetValueAsync(null).ConfigureAwait(false));
            }

            testDir.Delete(true);
        }

        [Test]
        public void GetValueAsync_GivenEmptyKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "getvalueasync_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;
            using (var cache = new DiskCache(testDir, cachePolicy, size))
            {
                Assert.ThrowsAsync<ArgumentNullException>(async () => await cache.GetValueAsync(string.Empty).ConfigureAwait(false));
            }

            testDir.Delete(true);
        }

        [Test]
        public void GetValueAsync_GivenWhiteSpaceKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "getvalueasync_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;
            using (var cache = new DiskCache(testDir, cachePolicy, size))
            {
                Assert.ThrowsAsync<ArgumentNullException>(async () => await cache.GetValueAsync("  ").ConfigureAwait(false));
            }

            testDir.Delete(true);
        }











        [Test]
        public void TryGetValue_GivenNullKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trygetvalue_out_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;
            using (var cache = new DiskCache(testDir, cachePolicy, size))
            {
                Assert.Throws<ArgumentNullException>(() => cache.TryGetValue(null, out var tmp));
            }

            testDir.Delete(true);
        }

        [Test]
        public void TryGetValue_GivenEmptyKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trygetvalue_out_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;
            using (var cache = new DiskCache(testDir, cachePolicy, size))
            {
                Assert.Throws<ArgumentNullException>(() => cache.TryGetValue(string.Empty, out var tmp));
            }

            testDir.Delete(true);
        }

        [Test]
        public void TryGetValue_GivenWhiteSpaceKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trygetvalue_out_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;
            using (var cache = new DiskCache(testDir, cachePolicy, size))
            {
                Assert.Throws<ArgumentNullException>(() => cache.TryGetValue("  ", out var tmp));
            }

            testDir.Delete(true);
        }

        [Test]
        public void TryGetValueTuple_GivenNullKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trygetvalue_tuple_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;
            using (var cache = new DiskCache(testDir, cachePolicy, size))
            {
                Assert.Throws<ArgumentNullException>(() => cache.TryGetValue(null));
            }

            testDir.Delete(true);
        }

        [Test]
        public void TryGetValueTuple_GivenEmptyKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trygetvalue_tuple_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;
            using (var cache = new DiskCache(testDir, cachePolicy, size))
            {
                Assert.Throws<ArgumentNullException>(() => cache.TryGetValue(string.Empty));
            }

            testDir.Delete(true);
        }

        [Test]
        public void TryGetValueTuple_GivenWhiteSpaceKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trygetvalue_tuple_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;
            using (var cache = new DiskCache(testDir, cachePolicy, size))
            {
                Assert.Throws<ArgumentNullException>(() => cache.TryGetValue("  "));
            }

            testDir.Delete(true);
        }

        [Test]
        public void TryGetValueAsyncTuple_GivenNullKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trygetvalueasync_tuple_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;
            using (var cache = new DiskCache(testDir, cachePolicy, size))
            {
                Assert.ThrowsAsync<ArgumentNullException>(async () => await cache.TryGetValueAsync(null).ConfigureAwait(false));
            }

            testDir.Delete(true);
        }

        [Test]
        public void TryGetValueAsyncTuple_GivenEmptyKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trygetvalueasync_tuple_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;
            using (var cache = new DiskCache(testDir, cachePolicy, size))
            {
                Assert.ThrowsAsync<ArgumentNullException>(async () => await cache.TryGetValueAsync(string.Empty).ConfigureAwait(false));
            }

            testDir.Delete(true);
        }

        [Test]
        public void TryGetValueAsyncTuple_GivenWhiteSpaceKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trygetvalueasync_tuple_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;
            using (var cache = new DiskCache(testDir, cachePolicy, size))
            {
                Assert.ThrowsAsync<ArgumentNullException>(async () => await cache.TryGetValueAsync("  ").ConfigureAwait(false));
            }

            testDir.Delete(true);
        }

        [Test]
        public void ContainsKey_WhenEmpty_ReturnsFalse()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "containskey_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;
            using (var cache = new DiskCache(testDir, cachePolicy, size))
            {
                Assert.IsFalse(cache.ContainsKey("asd"));
            }

            testDir.Delete(true);
        }

        [Test]
        public async Task ContainsKeyAsync_WhenEmpty_ReturnsFalse()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "containskeyasync_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;
            using (var cache = new DiskCache(testDir, cachePolicy, size))
            {
                var containsKey = await cache.ContainsKeyAsync("asd").ConfigureAwait(false);
                Assert.IsFalse(containsKey);
            }

            testDir.Delete(true);
        }

        [Test]
        public void GetValue_WhenEmpty_ThrowsKeyNotFoundEx()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "getvalue_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;
            using (var cache = new DiskCache(testDir, cachePolicy, size))
            {
                Assert.Throws<KeyNotFoundException>(() => cache.GetValue("asd"));
            }

            testDir.Delete(true);
        }

        [Test]
        public void GetValueAsync_WhenEmpty_ThrowsKeyNotFoundEx()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "getvalueasync_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;
            using (var cache = new DiskCache(testDir, cachePolicy, size))
            {
                Assert.ThrowsAsync<KeyNotFoundException>(async () => await cache.GetValueAsync("asd").ConfigureAwait(false));
            }

            testDir.Delete(true);
        }

        [Test]
        public void TryGetValue_WhenEmpty_ReturnsFalseValueOutNull()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trygetvalue_out_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;
            using (var cache = new DiskCache(testDir, cachePolicy, size))
            {
                var result = cache.TryGetValue("asd", out var stream);
                Assert.Multiple(() =>
                {
                    Assert.IsFalse(result);
                    Assert.IsNull(stream);
                });
            }

            testDir.Delete(true);
        }

        [Test]
        public void TryGetValue_WhenEmpty_ReturnsFalseValueNullStreamTuple()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trygetvalue_tuple_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;
            using (var cache = new DiskCache(testDir, cachePolicy, size))
            {
                var result = cache.TryGetValue("asd");
                Assert.Multiple(() =>
                {
                    Assert.IsFalse(result.hasValue);
                    Assert.IsNull(result.stream);
                });
            }

            testDir.Delete(true);
        }

        [Test]
        public async Task TryGetValueAsync_WhenEmpty_ReturnsFalseValueNullStreamTuple()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trygetvalueasync_tuple_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy>();
            const ulong size = 123;
            using (var cache = new DiskCache(testDir, cachePolicy, size))
            {
                var result = await cache.TryGetValueAsync("asd").ConfigureAwait(false);
                Assert.Multiple(() =>
                {
                    Assert.IsFalse(result.hasValue);
                    Assert.IsNull(result.stream);
                });
            }

            testDir.Delete(true);
        }
    }
}
