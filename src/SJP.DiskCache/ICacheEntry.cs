using System;

namespace SJP.DiskCache
{
    /// <summary>
    /// Represents a generic cache entry, including information useful for cache policies.
    /// </summary>
    /// <typeparam name="TKey">The type of keys used in the cache.</typeparam>
    public interface ICacheEntry<out TKey>
    {
        /// <summary>
        /// The key that the entry represents when looking up in the cache.
        /// </summary>
        TKey Key { get; }

        /// <summary>
        /// The size of the data that the cache entry is associated with.
        /// </summary>
        ulong Size { get; }

        /// <summary>
        /// The last time at which the entry was retrieved from the cache.
        /// </summary>
        DateTime LastAccessed { get; }

        /// <summary>
        /// When the cache entry was created.
        /// </summary>
        DateTime CreationTime { get; }

        /// <summary>
        /// The number of times that the cache entry has been accessed.
        /// </summary>
        ulong AccessCount { get; }

        /// <summary>
        /// Refreshes the cache entry, primarily to acknowledge an access.
        /// </summary>
        void Refresh();
    }
}