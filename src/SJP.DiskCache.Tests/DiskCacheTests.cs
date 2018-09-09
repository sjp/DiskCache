using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace SJP.DiskCache.Tests
{
    [TestFixture]
    internal static class DiskCacheTests
    {
        [Test]
        public static void Ctor_GivenNullDirectory_ThrowsArgNullException()
        {
            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            Assert.Throws<ArgumentNullException>(() => new DiskCache<string>((DirectoryInfo)null, cachePolicy, 123));
        }

        [Test]
        public static void Ctor_GivenNullCachePolicy_ThrowsArgNullException()
        {
            var dir = new DirectoryInfo(Environment.CurrentDirectory);
            Assert.Throws<ArgumentNullException>(() => new DiskCache<string>(dir, null, 123));
        }

        [Test]
        public static void Ctor_GivenZeroStorageCapacity_ThrowsArgOutOfRangeException()
        {
            var dir = new DirectoryInfo(Environment.CurrentDirectory);
            var cachePolicy = Mock.Of<ICachePolicy<string>>();

            Assert.Throws<ArgumentOutOfRangeException>(() => new DiskCache<string>(dir, cachePolicy, 0));
        }

        [Test]
        public static void Ctor_GivenNegativeInterval_ThrowsArgOutOfRangeException()
        {
            var dir = new DirectoryInfo(Environment.CurrentDirectory);
            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 123;

            Assert.Throws<ArgumentOutOfRangeException>(() => new DiskCache<string>(dir, cachePolicy, size, TimeSpan.FromSeconds(-1)));
        }

        [Test]
        public static void Ctor_GivenZeroInterval_ThrowsArgOutOfRangeException()
        {
            var dir = new DirectoryInfo(Environment.CurrentDirectory);
            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 123;

            Assert.Throws<ArgumentOutOfRangeException>(() => new DiskCache<string>(dir, cachePolicy, size, TimeSpan.FromSeconds(0)));
        }

        [Test]
        public static void Ctor_GivenNullDirectoryPath_ThrowsArgNullException()
        {
            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 123;

            Assert.Throws<ArgumentNullException>(() => new DiskCache<string>((string)null, cachePolicy, size));
        }

        [Test]
        public static void Ctor_GivenEmptyDirectoryPath_ThrowsArgNullException()
        {
            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 123;

            Assert.Throws<ArgumentNullException>(() => new DiskCache<string>(string.Empty, cachePolicy, size));
        }

        [Test]
        public static void Ctor_GivenWhiteSpaceDirectoryPath_ThrowsArgNullException()
        {
            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 123;

            Assert.Throws<ArgumentNullException>(() => new DiskCache<string>("   ", cachePolicy, size));
        }

        [Test]
        public static void Ctor_GivenNonExistentDirectory_ThrowsDirNotFoundException()
        {
            var dir = Path.Combine(Environment.CurrentDirectory, "asdasdasd");
            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 123;

            Assert.Throws<DirectoryNotFoundException>(() => new DiskCache<string>(dir, cachePolicy, size));
        }

        [Test]
        public static void ContainsKey_GivenNullKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "containskey_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 123;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                Assert.Throws<ArgumentNullException>(() => cache.ContainsKey(null));
            }

            testDir.Delete(true);
        }

        [Test]
        public static void ContainsKeyAsync_GivenNullKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "containskeyasync_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 123;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                Assert.ThrowsAsync<ArgumentNullException>(async () => await cache.ContainsKeyAsync(null).ConfigureAwait(false));
            }

            testDir.Delete(true);
        }

        [Test]
        public static void GetValue_GivenNullKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "getvalue_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 123;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                Assert.Throws<ArgumentNullException>(() => cache.GetValue(null));
            }

            testDir.Delete(true);
        }

        [Test]
        public static void GetValueAsync_GivenNullKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "getvalueasync_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 123;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                Assert.ThrowsAsync<ArgumentNullException>(async () => await cache.GetValueAsync(null).ConfigureAwait(false));
            }

            testDir.Delete(true);
        }

        [Test]
        public static void TryGetValue_GivenNullKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trygetvalue_out_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 123;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                Assert.Throws<ArgumentNullException>(() => cache.TryGetValue(null, out var tmp));
            }

            testDir.Delete(true);
        }

        [Test]
        public static void TryGetValueTuple_GivenNullKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trygetvalue_tuple_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 123;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                Assert.Throws<ArgumentNullException>(() => cache.TryGetValue(null));
            }

            testDir.Delete(true);
        }

        [Test]
        public static void TryGetValueAsyncTuple_GivenNullKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trygetvalueasync_tuple_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 123;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                Assert.ThrowsAsync<ArgumentNullException>(async () => await cache.TryGetValueAsync(null).ConfigureAwait(false));
            }

            testDir.Delete(true);
        }

        [Test]
        public static void ContainsKey_WhenEmpty_ReturnsFalse()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "containskey_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 123;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                Assert.IsFalse(cache.ContainsKey("asd"));
            }

            testDir.Delete(true);
        }

        [Test]
        public static async Task ContainsKeyAsync_WhenEmpty_ReturnsFalse()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "containskeyasync_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 123;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                var containsKey = await cache.ContainsKeyAsync("asd").ConfigureAwait(false);
                Assert.IsFalse(containsKey);
            }

            testDir.Delete(true);
        }

        [Test]
        public static void GetValue_WhenEmpty_ThrowsKeyNotFoundEx()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "getvalue_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 123;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                Assert.Throws<KeyNotFoundException>(() => cache.GetValue("asd"));
            }

            testDir.Delete(true);
        }

        [Test]
        public static void GetValueAsync_WhenEmpty_ThrowsKeyNotFoundEx()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "getvalueasync_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 123;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                Assert.ThrowsAsync<KeyNotFoundException>(async () => await cache.GetValueAsync("asd").ConfigureAwait(false));
            }

            testDir.Delete(true);
        }

        [Test]
        public static void TryGetValue_WhenEmpty_ReturnsFalseValueOutNull()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trygetvalue_out_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 123;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
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
        public static void TryGetValue_WhenEmpty_ReturnsFalseValueNullStreamTuple()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trygetvalue_tuple_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 123;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                var (hasValue, stream) = cache.TryGetValue("asd");
                Assert.Multiple(() =>
                {
                    Assert.IsFalse(hasValue);
                    Assert.IsNull(stream);
                });
            }

            testDir.Delete(true);
        }

        [Test]
        public static async Task TryGetValueAsync_WhenEmpty_ReturnsFalseValueNullStreamTuple()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trygetvalueasync_tuple_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 123;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                var (hasValue, stream) = await cache.TryGetValueAsync("asd").ConfigureAwait(false);
                Assert.Multiple(() =>
                {
                    Assert.IsFalse(hasValue);
                    Assert.IsNull(stream);
                });
            }

            testDir.Delete(true);
        }

        [Test]
        public static void SetValue_WhenGivenNullKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "setvalue_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 123;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                Assert.Throws<ArgumentNullException>(() => cache.SetValue(null, Stream.Null));
            }

            testDir.Delete(true);
        }

        [Test]
        public static void SetValue_WhenGivenNullStream_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "setvalue_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 123;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                Assert.Throws<ArgumentNullException>(() => cache.SetValue("asd", null));
            }

            testDir.Delete(true);
        }

        [Test]
        public static void SetValue_WhenGivenUnreadableStream_ThrowsArgException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "setvalue_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            var stream = new Mock<Stream>();
            stream.Setup(s => s.CanRead).Returns(false);
            const ulong size = 123;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                Assert.Throws<ArgumentException>(() => cache.SetValue("asd", stream.Object));
            }

            testDir.Delete(true);
        }

        [Test]
        public static void TrySetValue_WhenGivenNullKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trysetvalue_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 123;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                Assert.Throws<ArgumentNullException>(() => cache.TrySetValue(null, Stream.Null));
            }

            testDir.Delete(true);
        }

        [Test]
        public static void TrySetValue_WhenGivenNullStream_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trysetvalue_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 123;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                Assert.Throws<ArgumentNullException>(() => cache.TrySetValue("asd", null));
            }

            testDir.Delete(true);
        }

        [Test]
        public static void TrySetValue_WhenGivenUnreadableStream_ThrowsArgException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trysetvalue_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            var stream = new Mock<Stream>();
            stream.Setup(s => s.CanRead).Returns(false);
            const ulong size = 123;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                Assert.Throws<ArgumentException>(() => cache.TrySetValue("asd", stream.Object));
            }

            testDir.Delete(true);
        }

        [Test]
        public static void SetValueAsync_WhenGivenNullKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "setvalueasync_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 123;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                Assert.ThrowsAsync<ArgumentNullException>(async () => await cache.SetValueAsync(null, Stream.Null).ConfigureAwait(false));
            }

            testDir.Delete(true);
        }

        [Test]
        public static void SetValueAsync_WhenGivenNullStream_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "setvalueasync_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 123;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                Assert.ThrowsAsync<ArgumentNullException>(async () => await cache.SetValueAsync("asd", null).ConfigureAwait(false));
            }

            testDir.Delete(true);
        }

        [Test]
        public static void SetValueAsync_WhenGivenUnreadableStream_ThrowsArgException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "setvalueasync_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            var stream = new Mock<Stream>();
            stream.Setup(s => s.CanRead).Returns(false);
            const ulong size = 123;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                Assert.ThrowsAsync<ArgumentException>(async () => await cache.SetValueAsync("asd", stream.Object).ConfigureAwait(false));
            }

            testDir.Delete(true);
        }

        [Test]
        public static void TrySetValueAsync_WhenGivenNullKey_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trysetvalueasync_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 123;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                Assert.ThrowsAsync<ArgumentNullException>(async () => await cache.TrySetValueAsync(null, Stream.Null).ConfigureAwait(false));
            }

            testDir.Delete(true);
        }

        [Test]
        public static void TrySetValueAsync_WhenGivenNullStream_ThrowsArgNullException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trysetvalueasync_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 123;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                Assert.ThrowsAsync<ArgumentNullException>(async () => await cache.TrySetValueAsync("asd", null).ConfigureAwait(false));
            }

            testDir.Delete(true);
        }

        [Test]
        public static void TrySetValueAsync_WhenGivenUnreadableStream_ThrowsArgException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trysetvalueasync_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            var stream = new Mock<Stream>();
            stream.Setup(s => s.CanRead).Returns(false);
            const ulong size = 123;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                Assert.ThrowsAsync<ArgumentException>(async () => await cache.TrySetValueAsync("asd", stream.Object).ConfigureAwait(false));
            }

            testDir.Delete(true);
        }

        [Test]
        public static void SetValue_WhenGivenValueTooLarge_ThrowsArgException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "setvalue_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 2;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                Assert.Throws<ArgumentException>(() => cache.SetValue("asd", new MemoryStream(new byte[4])));
            }

            testDir.Delete(true);
        }

        [Test]
        public static void TrySetValue_WhenGivenValueTooLarge_ThrowsArgException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trysetvalue_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 2;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                var result = cache.TrySetValue("asd", new MemoryStream(new byte[4]));
                Assert.IsFalse(result);
            }

            testDir.Delete(true);
        }

        [Test]
        public static void SetValueAsync_WhenGivenValueTooLarge_ThrowsArgException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "setvalueasync_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 2;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                Assert.ThrowsAsync<ArgumentException>(async () => await cache.SetValueAsync("asd", new MemoryStream(new byte[4])).ConfigureAwait(false));
            }

            testDir.Delete(true);
        }

        [Test]
        public static async Task TrySetValueAsync_WhenGivenValueTooLarge_ThrowsArgException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trysetvalueasync_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            const ulong size = 2;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                var result = await cache.TrySetValueAsync("asd", new MemoryStream(new byte[4])).ConfigureAwait(false);
                Assert.IsFalse(result);
            }

            testDir.Delete(true);
        }

        [Test]
        public static void GetSetValue_WhenInvokedTogether_HasEqualInputAndOutput()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "getsetvalue_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            var input = new byte[] { 1, 2, 3, 4 };
            const ulong size = 20;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                cache.SetValue("asd", new MemoryStream(input));
                var result = cache.GetValue("asd");
                using (var reader = new BinaryReader(result))
                {
                    var resultBytes = reader.ReadBytes(4);
                    var seqEqual = input.SequenceEqual(resultBytes);
                    Assert.IsTrue(seqEqual);
                }
            }

            testDir.Delete(true);
        }

        [Test]
        public static void TryGetSetValue_WhenInvokedTogether_ReturnsTrueAndHasEqualInputAndOutput()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trygetsetvalue_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            var input = new byte[] { 1, 2, 3, 4 };
            const ulong size = 20;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                var setSuccess = cache.TrySetValue("asd", new MemoryStream(input));
                var getSuccess = cache.TryGetValue("asd", out var result);

                using (var reader = new BinaryReader(result))
                {
                    var resultBytes = reader.ReadBytes(4);
                    var seqEqual = input.SequenceEqual(resultBytes);
                    Assert.IsTrue(seqEqual);
                }
            }

            testDir.Delete(true);
        }

        [Test]
        public static async Task GetSetValueAsync_WhenInvokedTogether_HasEqualInputAndOutput()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "getsetvalueasync_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            var input = new byte[] { 1, 2, 3, 4 };
            const ulong size = 20;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                await cache.SetValueAsync("asd", new MemoryStream(input)).ConfigureAwait(false);
                var result = await cache.GetValueAsync("asd").ConfigureAwait(false);
                using (var reader = new BinaryReader(result))
                {
                    var resultBytes = reader.ReadBytes(4);
                    var seqEqual = input.SequenceEqual(resultBytes);
                    Assert.IsTrue(seqEqual);
                }
            }

            testDir.Delete(true);
        }

        [Test]
        public static async Task TryGetSetValueAsync_WhenInvokedTogether_ReturnsTrueAndHasEqualInputAndOutput()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trygetsetvalueasync_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            var input = new byte[] { 1, 2, 3, 4 };
            const ulong size = 20;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                var setSuccess = await cache.TrySetValueAsync("asd", new MemoryStream(input)).ConfigureAwait(false);
                var (hasValue, stream) = await cache.TryGetValueAsync("asd").ConfigureAwait(false);

                using (var reader = new BinaryReader(stream))
                {
                    var resultBytes = reader.ReadBytes(4);
                    var seqEqual = input.SequenceEqual(resultBytes);
                    Assert.IsTrue(seqEqual);
                }
            }

            testDir.Delete(true);
        }

        [Test]
        public static void GetSetValue_WhenInvokedTogetherUpdatingValue_HasEqualInputAndOutput()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "getsetvalue_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            var input = new byte[] { 1, 2, 3, 4 };
            var updatedInput = new byte[] { 3, 4, 5, 6 };
            const ulong size = 20;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                cache.SetValue("asd", new MemoryStream(input));
                cache.SetValue("asd", new MemoryStream(updatedInput));

                var result = cache.GetValue("asd");
                using (var reader = new BinaryReader(result))
                {
                    var resultBytes = reader.ReadBytes(4);
                    var seqEqual = updatedInput.SequenceEqual(resultBytes);
                    Assert.IsTrue(seqEqual);
                }
            }

            testDir.Delete(true);
        }

        [Test]
        public static void TryGetSetValue_WhenInvokedTogetherUpdatingValue_ReturnsTrueAndHasEqualInputAndOutput()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trygetsetvalue_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            var input = new byte[] { 1, 2, 3, 4 };
            var updatedInput = new byte[] { 3, 4, 5, 6 };
            const ulong size = 20;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                var setSuccess = cache.TrySetValue("asd", new MemoryStream(input));
                setSuccess = cache.TrySetValue("asd", new MemoryStream(updatedInput));
                var getSuccess = cache.TryGetValue("asd", out var result);

                using (var reader = new BinaryReader(result))
                {
                    var resultBytes = reader.ReadBytes(4);
                    var seqEqual = updatedInput.SequenceEqual(resultBytes);
                    Assert.IsTrue(seqEqual);
                }
            }

            testDir.Delete(true);
        }

        [Test]
        public static async Task GetSetValueAsync_WhenInvokedTogetherUpdatingValue_HasEqualInputAndOutput()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "getsetvalueasync_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            var input = new byte[] { 1, 2, 3, 4 };
            var updatedInput = new byte[] { 3, 4, 5, 6 };
            const ulong size = 20;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                await cache.SetValueAsync("asd", new MemoryStream(input)).ConfigureAwait(false);
                await cache.SetValueAsync("asd", new MemoryStream(updatedInput)).ConfigureAwait(false);
                var result = await cache.GetValueAsync("asd").ConfigureAwait(false);
                using (var reader = new BinaryReader(result))
                {
                    var resultBytes = reader.ReadBytes(4);
                    var seqEqual = updatedInput.SequenceEqual(resultBytes);
                    Assert.IsTrue(seqEqual);
                }
            }

            testDir.Delete(true);
        }

        [Test]
        public static async Task TryGetSetValueAsync_WhenInvokedTogetherUpdatingValue_ReturnsTrueAndHasEqualInputAndOutput()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trygetsetvalueasync_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = Mock.Of<ICachePolicy<string>>();
            var input = new byte[] { 1, 2, 3, 4 };
            var updatedInput = new byte[] { 3, 4, 5, 6 };
            const ulong size = 20;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                var setSuccess = await cache.TrySetValueAsync("asd", new MemoryStream(input)).ConfigureAwait(false);
                setSuccess = await cache.TrySetValueAsync("asd", new MemoryStream(updatedInput)).ConfigureAwait(false);
                var (hasValue, stream) = await cache.TryGetValueAsync("asd").ConfigureAwait(false);

                using (var reader = new BinaryReader(stream))
                {
                    var resultBytes = reader.ReadBytes(4);
                    var seqEqual = updatedInput.SequenceEqual(resultBytes);
                    Assert.IsTrue(seqEqual);
                }
            }

            testDir.Delete(true);
        }

        [Test]
        public static void ContainsKey_WhenValueExpired_ReturnsFalse()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "containskey_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = new FixedTimespanCachePolicy<string>(TimeSpan.FromMilliseconds(1));
            var input = new byte[] { 1, 2, 3, 4 };
            const ulong size = 20;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size, TimeSpan.FromMilliseconds(20)))
            {
                cache.SetValue("asd", new MemoryStream(input));
                Task.Delay(100).Wait();
                Assert.IsFalse(cache.ContainsKey("asd"));
            }

            testDir.Delete(true);
        }

        [Test]
        public static void GetValue_WhenValueExpired_ThrowsKeyNotFoundException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "getvalueexpired_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = new FixedTimespanCachePolicy<string>(TimeSpan.FromMilliseconds(1));
            var input = new byte[] { 1, 2, 3, 4 };
            const ulong size = 20;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size, TimeSpan.FromMilliseconds(5)))
            {
                cache.SetValue("asd", new MemoryStream(input));
                Task.Delay(100).Wait();
                Assert.Throws<KeyNotFoundException>(() => cache.GetValue("asd"));
            }

            testDir.Delete(true);
        }

        [Test]
        public static void TryGetValue_WhenValueExpired_ReturnsFalseAndNullStream()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trygetvalueexpired_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = new FixedTimespanCachePolicy<string>(TimeSpan.FromMilliseconds(1));
            var input = new byte[] { 1, 2, 3, 4 };
            const ulong size = 20;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size, TimeSpan.FromMilliseconds(5)))
            {
                cache.SetValue("asd", new MemoryStream(input));
                Task.Delay(100).Wait();
                var result = cache.TryGetValue("asd", out var stream);

                Assert.IsFalse(result);
                Assert.IsNull(stream);
            }

            testDir.Delete(true);
        }

        [Test]
        public static void GetValueAsync_WhenValueExpired_ThrowsKeyNotFoundException()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "getvalueexpired_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = new FixedTimespanCachePolicy<string>(TimeSpan.FromMilliseconds(1));
            var input = new byte[] { 1, 2, 3, 4 };
            const ulong size = 20;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size, TimeSpan.FromMilliseconds(5)))
            {
                cache.SetValue("asd", new MemoryStream(input));
                Task.Delay(100).Wait();
                Assert.ThrowsAsync<KeyNotFoundException>(async () => await cache.GetValueAsync("asd").ConfigureAwait(false));
            }

            testDir.Delete(true);
        }

        [Test]
        public static async Task TryGetValueAsync_WhenValueExpired_ReturnsFalseAndNullStream()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "trygetvalueexpired_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = new FixedTimespanCachePolicy<string>(TimeSpan.FromMilliseconds(1));
            var input = new byte[] { 1, 2, 3, 4 };
            const ulong size = 20;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size, TimeSpan.FromMilliseconds(5)))
            {
                await cache.SetValueAsync("asd", new MemoryStream(input)).ConfigureAwait(false);
                await Task.Delay(100).ConfigureAwait(false);
                var (hasValue, stream) = await cache.TryGetValueAsync("asd").ConfigureAwait(false);

                Assert.IsFalse(hasValue);
                Assert.IsNull(stream);
            }

            testDir.Delete(true);
        }

        [Test]
        public static void Clear_WhenValuePresent_RemovesAnyPresentValues()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "clearasync_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = new FifoCachePolicy<string>();
            var input = new byte[] { 1, 2, 3, 4 };
            const ulong size = 20;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                cache.SetValue("asd", new MemoryStream(input));
                cache.Clear();

                var result = cache.ContainsKey("asd");
                Assert.IsFalse(result);
            }

            testDir.Delete(true);
        }

        [Test]
        public static async Task ClearAsync_WhenValuePresent_RemovesAnyPresentValues()
        {
            var testDirPath = Path.Combine(Environment.CurrentDirectory, "clearasync_test");
            var testDir = new DirectoryInfo(testDirPath);
            if (!testDir.Exists)
                testDir.Create();

            var cachePolicy = new FifoCachePolicy<string>();
            var input = new byte[] { 1, 2, 3, 4 };
            const ulong size = 20;
            using (var cache = new DiskCache<string>(testDir, cachePolicy, size))
            {
                await cache.SetValueAsync("asd", new MemoryStream(input)).ConfigureAwait(false);
                await cache.ClearAsync().ConfigureAwait(false);

                var result = await cache.ContainsKeyAsync("asd").ConfigureAwait(false);
                Assert.IsFalse(result);
            }

            testDir.Delete(true);
        }
    }
}
