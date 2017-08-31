using System;
using System.Collections.Generic;
using System.Linq;

namespace SJP.DiskCache
{
    /// <summary>
    /// Evicts values in a cache that are Least Recently Used (LRU).
    /// </summary>
    public class LruCachePolicy : ICachePolicy
    {
        /// <summary>
        /// Retrives the set of entries that are now expired in the cache.
        /// </summary>
        /// <param name="entries">The set of cache entries to evaluate.</param>
        /// <param name="maximumStorageCapacity">The maximum size of the disk cache. Useful for determining ordering of cache entries.</param>
        /// <returns>A collection of entries that should be evicted from the cache.</returns>
        public IEnumerable<ICacheEntry> GetExpiredEntries(IEnumerable<ICacheEntry> entries, ulong maximumStorageCapacity)
        {
            if (entries == null)
                throw new ArgumentNullException(nameof(entries));

            ulong totalSum = 0;
            var validKeys = entries
                .OrderByDescending(e => e.LastAccessed)
                .TakeWhile(e =>
                {
                    totalSum += e.Size;
                    return totalSum <= maximumStorageCapacity;
                })
                .Select(e => e.Key)
                .ToList();

            var validKeySet = new HashSet<string>(validKeys);
            return entries.Where(e => !validKeySet.Contains(e.Key)).ToList();
        }
    }
}