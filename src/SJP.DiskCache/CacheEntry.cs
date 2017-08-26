using System;
using System.Threading;

namespace SJP.DiskCache
{
    public class CacheEntry : ICacheEntry
    {
        public CacheEntry(string key, ulong size)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (size == 0)
                throw new ArgumentException("The file size must be non-zero.", nameof(size));

            Key = key;
            Size = size;
        }

        public string Key { get; }

        public ulong Size { get; }

        public DateTime LastAccessed => _lastAccessed;

        public DateTime CreationTime { get; } = DateTime.Now;

        public ulong AccessCount => Convert.ToUInt64(_accessCount);

        public void Refresh()
        {
            Interlocked.Increment(ref _accessCount);
            _lastAccessed = DateTime.Now;
        }

        private long _accessCount;
        private DateTime _lastAccessed = DateTime.Now;
    }
}