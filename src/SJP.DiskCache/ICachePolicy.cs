using System.Collections.Generic;

namespace SJP.DiskCache
{
    /// <summary>
    /// Represents a generic cache policy
    /// </summary>
    /// <typeparam name="TKey">The type of keys used in the cache.</typeparam>
    public interface ICachePolicy<TKey>
    {
        /// <summary>
        /// Retrieves the set of entries that are now expired in the cache.
        /// </summary>
        /// <param name="entries">The set of cache entries to evaluate.</param>
        /// <param name="maximumStorageCapacity">The maximum size of the disk cache. Useful for determining ordering of cache entries.</param>
        /// <returns>A collection of entries that should be evicted from the cache.</returns>
        IEnumerable<ICacheEntry<TKey>> GetExpiredEntries(IEnumerable<ICacheEntry<TKey>> entries, ulong maximumStorageCapacity);
    }
}