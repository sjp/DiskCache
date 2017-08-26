using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace SJP.DiskCache
{
    public class DiskCache : IDiskCache
    {
        public DiskCache(DirectoryInfo directory, ICachePolicy cachePolicy, ulong storageCapacity, TimeSpan? pollingInterval = null)
            : this(directory?.FullName ?? throw new ArgumentNullException(nameof(directory)), cachePolicy, storageCapacity, pollingInterval)
        {
        }

        public DiskCache(string directoryPath, ICachePolicy cachePolicy, ulong storageCapacity, TimeSpan? pollingInterval = null)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
                throw new ArgumentNullException(nameof(directoryPath));
            if (!Directory.Exists(directoryPath))
                throw new ArgumentException("The cache directory does not exit.", nameof(directoryPath));
            if (storageCapacity == 0)
                throw new ArgumentOutOfRangeException("The storage capacity must be at least 1 byte. Given: " + storageCapacity.ToString(), nameof(storageCapacity));

            CachePath = new DirectoryInfo(directoryPath);
            Policy = cachePolicy ?? throw new ArgumentNullException(nameof(cachePolicy));
            MaximumStorageCapacity = storageCapacity;
            PollingInterval = pollingInterval ?? TimeSpan.FromMinutes(1);
        }

        public ulong MaximumStorageCapacity { get; }

        public TimeSpan PollingInterval { get; }

        public ICachePolicy Policy { get; }

        protected DirectoryInfo CachePath { get; }

        public event EventHandler<object> EntryAdded;

        public event EventHandler<object> EntryUpdated;

        public event EventHandler<object> EntryRemoved;

        public void Clear()
        {
            foreach (var entry in _entryLookup)
            {
                File.Delete(_fileLookup[entry.Key]);
                _entryLookup.TryRemove(entry.Key, out var lookupEntry);
                EntryAdded?.Invoke(this, lookupEntry);
            }

            _entryLookup.Clear();
            _fileLookup.Clear();
        }

        public bool ContainsKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            return _entryLookup.ContainsKey(key);
        }

        public Task<bool> ContainsKeyAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            return Task.Run(() => ContainsKey(key));
        }

        public Stream GetValue(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (!_fileLookup.ContainsKey(key))
                throw new KeyNotFoundException($"Could not find a value for the key '{ key }'");
            var path = _fileLookup[key];
            if (!File.Exists(path))
                throw new FileNotFoundException($"Expected to find a path at the path '{ path }', but it does not exist.", path);

            return File.OpenRead(path);
        }

        public Task<Stream> GetValueAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            return Task.Run(() => GetValue(key));
        }

        public void SetValue(string key, Stream value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (!value.CanRead)
                throw new ArgumentException("The given stream is not readable.", nameof(value));

            var tmpFileName = Path.Combine(CachePath.FullName, Guid.NewGuid().ToString());
            using (var writer = File.OpenWrite(tmpFileName))
                value.CopyTo(writer);

            string cachePath = null;
            using (var tmpFileReader = File.OpenRead(tmpFileName))
                cachePath = GetPath(tmpFileReader);

            var isNew = ContainsKey(key);

            _fileLookup[key] = cachePath;
            var cacheEntry = (ICacheEntry)null;
            _entryLookup[key] = cacheEntry;

            if (isNew)
                EntryAdded.Invoke(this, cacheEntry);
            else
                EntryUpdated.Invoke(this, cacheEntry);
        }

        public bool TryGetValue(string key, out Stream stream)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            var hasValue = ContainsKey(key);
            stream = hasValue ? GetValue(key) : null;

            return hasValue;
        }

        public (bool hasValue, Stream stream) TryGetValue(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            var hasValue = ContainsKey(key);
            var stream = hasValue ? GetValue(key) : null;

            return (hasValue, stream);
        }

        public async Task<(bool hasValue, Stream stream)> TryGetValueAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            var hasValue = await ContainsKeyAsync(key).ConfigureAwait(false);
            Stream stream = null;
            if (hasValue)
                stream = await GetValueAsync(key).ConfigureAwait(false);

            return (hasValue, stream);
        }

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (!disposing)
                return;

            Clear();
            _disposed = true;
        }

        protected virtual string GetPath(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead)
                throw new ArgumentException("The given stream is not readable.", nameof(stream));

            var hash = GetStreamHash(stream);
            // have 2 levels of directories to speed up IO
            // use 2 chars for each level
            var firstDir = hash.Substring(0, 2);
            var secondDir = hash.Substring(2, 2);

            return Path.Combine(CachePath.FullName, firstDir, secondDir, hash);
        }

        protected static string GetStreamHash(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead)
                throw new ArgumentException("The given stream is not readable.", nameof(stream));

            var shaHasher = new SHA256Managed();
            var hash = shaHasher.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }

        private bool _disposed;

        private readonly ConcurrentDictionary<string, ICacheEntry> _entryLookup;
        private readonly ConcurrentDictionary<string, string> _fileLookup;
    }
}