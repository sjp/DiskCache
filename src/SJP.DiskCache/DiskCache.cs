using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Globalization;

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
                throw new ArgumentOutOfRangeException("The storage capacity must be at least 1 byte. Given: " + storageCapacity.ToString(CultureInfo.InvariantCulture), nameof(storageCapacity));

            CachePath = new DirectoryInfo(directoryPath);
            Policy = cachePolicy ?? throw new ArgumentNullException(nameof(cachePolicy));
            MaximumStorageCapacity = storageCapacity;
            PollingInterval = pollingInterval ?? TimeSpan.FromMinutes(1);

            // remove the contents of the cache dir to ensure that
            // the file size limits are tracked properly
            foreach (var dir in CachePath.EnumerateDirectories())
                dir.Delete(true);

            foreach (var file in CachePath.EnumerateFiles())
                file.Delete();
        }

        public ulong MaximumStorageCapacity { get; }

        public TimeSpan PollingInterval { get; }

        public ICachePolicy Policy { get; }

        protected DirectoryInfo CachePath { get; }

        public event EventHandler<ICacheEntry> EntryAdded;

        public event EventHandler<ICacheEntry> EntryUpdated;

        public event EventHandler<ICacheEntry> EntryRemoved;

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

            var cacheEntry = _entryLookup[key];
            cacheEntry.Refresh();
            _entryLookup[key] = cacheEntry;

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

            ulong totalBytesRead = 0;
            const long bufferSize = 4096;

            var tmpFileName = Path.Combine(CachePath.FullName, Guid.NewGuid().ToString());
            string hash = null;

            using (var shaHasher = new SHA256Managed())
            {
                using (var writer = File.OpenWrite(tmpFileName))
                {
                    byte[] oldBuffer;
                    int oldBytesRead;

                    var buffer = new byte[bufferSize];
                    var bytesRead = value.Read(buffer, 0, buffer.Length);
                    totalBytesRead += Convert.ToUInt32(bytesRead);

                    do
                    {
                        oldBytesRead = bytesRead;
                        oldBuffer = buffer;

                        buffer = new byte[bufferSize];
                        bytesRead = value.Read(buffer, 0, buffer.Length);
                        totalBytesRead += Convert.ToUInt32(bytesRead);

                        if (bytesRead == 0)
                        {
                            shaHasher.TransformFinalBlock(oldBuffer, 0, oldBytesRead);
                            writer.Write(oldBuffer, 0, oldBytesRead);
                        }
                        else
                        {
                            shaHasher.TransformBlock(oldBuffer, 0, oldBytesRead, oldBuffer, 0);
                            writer.Write(oldBuffer, 0, oldBytesRead);
                        }
                    }
                    while (bytesRead != 0 && totalBytesRead <= MaximumStorageCapacity);
                }

                if (totalBytesRead > MaximumStorageCapacity)
                {
                    File.Delete(tmpFileName); // remove the file, we can't keep it anyway
                    throw new ArgumentException("The given stream received data that was larger than the allotted storage capacity of " + MaximumStorageCapacity.ToString(CultureInfo.InvariantCulture), nameof(value));
                }

                var shaHashBytes = shaHasher.Hash;
                hash = BitConverter.ToString(shaHashBytes).Replace("-", string.Empty);
            }

            var cachePath = GetPath(hash);

            var isNew = ContainsKey(key);

            _fileLookup[key] = cachePath;
            var cacheEntry = (ICacheEntry)null;
            _entryLookup[key] = cacheEntry;

            if (isNew)
                EntryAdded.Invoke(this, cacheEntry);
            else
                EntryUpdated.Invoke(this, cacheEntry);

            ApplyCachePolicy();
        }

        public bool TrySetValue(string key, Stream value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (!value.CanRead)
                throw new ArgumentException("The given stream is not readable.", nameof(value));

            ulong totalBytesRead = 0;
            const long bufferSize = 4096;

            var tmpFileName = Path.Combine(CachePath.FullName, Guid.NewGuid().ToString());
            string hash = null;

            using (var shaHasher = new SHA256Managed())
            {
                using (var writer = File.OpenWrite(tmpFileName))
                {
                    byte[] oldBuffer;
                    int oldBytesRead;

                    var buffer = new byte[bufferSize];
                    var bytesRead = value.Read(buffer, 0, buffer.Length);
                    totalBytesRead += Convert.ToUInt32(bytesRead);

                    do
                    {
                        oldBytesRead = bytesRead;
                        oldBuffer = buffer;

                        buffer = new byte[bufferSize];
                        bytesRead = value.Read(buffer, 0, buffer.Length);
                        totalBytesRead += Convert.ToUInt32(bytesRead);

                        if (bytesRead == 0)
                        {
                            shaHasher.TransformFinalBlock(oldBuffer, 0, oldBytesRead);
                            writer.Write(oldBuffer, 0, oldBytesRead);
                        }
                        else
                        {
                            shaHasher.TransformBlock(oldBuffer, 0, oldBytesRead, oldBuffer, 0);
                            writer.Write(oldBuffer, 0, oldBytesRead);
                        }
                    }
                    while (bytesRead != 0 && totalBytesRead <= MaximumStorageCapacity);
                }

                if (totalBytesRead > MaximumStorageCapacity)
                {
                    File.Delete(tmpFileName); // remove the file, we can't keep it anyway
                    return false;
                }

                var shaHashBytes = shaHasher.Hash;
                hash = BitConverter.ToString(shaHashBytes).Replace("-", string.Empty);
            }

            var cachePath = GetPath(hash);
            File.Move(tmpFileName, cachePath);

            var isNew = ContainsKey(key);
            var fileInfo = new FileInfo(cachePath);
            var cacheEntry = new CacheEntry(key, Convert.ToUInt64(fileInfo.Length));
            _entryLookup[key] = cacheEntry;
            _fileLookup[key] = cachePath;

            if (isNew)
                EntryAdded?.Invoke(this, cacheEntry);
            else
                EntryUpdated?.Invoke(this, cacheEntry);

            ApplyCachePolicy();
            return true;
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

        protected void ApplyCachePolicy()
        {
            var expiredEntries = Policy.GetExpiredEntries(_entryLookup.Values, MaximumStorageCapacity);
            foreach (var expiredEntry in expiredEntries)
            {
                var key = expiredEntry.Key;
                var filePath = _fileLookup[key];
                File.Delete(filePath);
                _fileLookup.TryRemove(key, out var tmpFilePath);
                _entryLookup.TryRemove(key, out var lookupEntry);
                EntryRemoved?.Invoke(this, lookupEntry);
            }
        }

        protected virtual string GetPath(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
                throw new ArgumentNullException(nameof(hash));
            if (hash.Length != 32)
                throw new ArgumentException("The hash must be a 32 character long representation of a 256-bit hash.", nameof(hash));
            var allValidChars = hash.All(IsValidHexChar);
            if (!allValidChars)
                throw new ArgumentException("The hash must be string containing only hexadecimal characters that represent a 256-bit hash", nameof(hash));

            var firstDir = hash.Substring(0, 2);
            var secondDir = hash.Substring(2, 2);

            return Path.Combine(CachePath.FullName, firstDir, secondDir, hash);
        }

        protected static bool IsValidHexChar(char c) => byte.TryParse(c.ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var tmp);

        private bool _disposed;

        private readonly ConcurrentDictionary<string, ICacheEntry> _entryLookup = new ConcurrentDictionary<string, ICacheEntry>();
        private readonly ConcurrentDictionary<string, string> _fileLookup = new ConcurrentDictionary<string, string>();
    }
}