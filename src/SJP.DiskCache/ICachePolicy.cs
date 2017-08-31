using System.Collections.Generic;

namespace SJP.DiskCache
{
    /// <summary>
    /// Represents a generic cache policy
    /// </summary>
    public interface ICachePolicy
    {
        /// <summary>
        /// Retrives the set of entries that are now expired in the cache.
        /// </summary>
        /// <param name="entries">The set of cache entries to evaluate.</param>
        /// <param name="maximumStorageCapacity">The maximum size of the disk cache. Useful for determining ordering of cache entries.</param>
        /// <returns>A collection of entries that should be evicted from the cache.</returns>
        IEnumerable<ICacheEntry> GetExpiredEntries(IEnumerable<ICacheEntry> entries, ulong maximumStorageCapacity);
    }
}