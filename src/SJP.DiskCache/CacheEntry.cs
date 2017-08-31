using System;
using System.Diagnostics;
using System.Threading;

namespace SJP.DiskCache
{
    /// <summary>
    /// A cache entry to be used for assisting with accessing and applying cache policies within a disk cache.
    /// </summary>
    public class CacheEntry : ICacheEntry
    {
        /// <summary>
        /// Initializes a cache entry.
        /// </summary>
        /// <param name="key">The key of the cache entry that is used to retrieve data from a disk cache.</param>
        /// <param name="size">The size (on disk) of the data that the key is associated with.</param>
        public CacheEntry(string key, ulong size)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (size == 0)
                throw new ArgumentException("The file size must be non-zero.", nameof(size));

            Key = key;
            Size = size;

            // using a stopwatch instead of a raw datetime.
            // the reason for this is to handle changes in system time and time zone
            // without reporting incorrect values.
            _lastAccessedTimer = new Stopwatch();
            _lastAccessedTimer.Start();
        }

        /// <summary>
        /// The key that the entry represents when looking up in the cache.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// The size of the data that the cache entry is associated with.
        /// </summary>
        public ulong Size { get; }

        /// <summary>
        /// The last time at which the entry was retrieved from the cache.
        /// </summary>
        public DateTime LastAccessed => DateTime.Now - TimeSpan.FromTicks(_lastAccessedTimer.ElapsedTicks);

        /// <summary>
        /// When the cache entry was created.
        /// </summary>
        public DateTime CreationTime { get; } = DateTime.Now;

        /// <summary>
        /// The number of times that the cache entry has been accessed.
        /// </summary>
        public ulong AccessCount => Convert.ToUInt64(_accessCount);

        /// <summary>
        /// Refreshes the cache entry, primarily to acknowledge an access. Increments access count and restarts access timer.
        /// </summary>
        public void Refresh()
        {
            Interlocked.Increment(ref _accessCount);
            _lastAccessedTimer.Restart();
        }

        private long _accessCount;
        private readonly Stopwatch _lastAccessedTimer;
    }
}