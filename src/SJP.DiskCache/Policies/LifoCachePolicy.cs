using System;
using System.Collections.Generic;
using System.Linq;

namespace SJP.DiskCache
{
    /// <summary>
    /// Evicts values in a cache that were introduced last. Last-In-First-Out (LIFO).
    /// </summary>
    /// <typeparam name="TKey">The type of keys used in the cache.</typeparam>
    public class LifoCachePolicy<TKey> : ICachePolicy<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Initializes a last-in-first-out cache policy.
        /// </summary>
        /// <param name="keyComparer">The <see cref="IEqualityComparer{TKey}"/> implementation to use when comparing cache keys, or <c>null</c> to use the default <see cref="EqualityComparer{TKey}"/> implementation for the set type.</param>
        public LifoCachePolicy(IEqualityComparer<TKey> keyComparer = null)
        {
            KeyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
        }

        /// <summary>
        /// The <see cref="IEqualityComparer{TKey}"/> implementation to use when comparing cache keys.
        /// </summary>
        protected IEqualityComparer<TKey> KeyComparer { get; }

        /// <summary>
        /// Retrieves the set of entries that are now expired in the cache.
        /// </summary>
        /// <param name="entries">The set of cache entries to evaluate.</param>
        /// <param name="maximumStorageCapacity">The maximum size of the disk cache. Useful for determining ordering of cache entries.</param>
        /// <returns>A collection of entries that should be evicted from the cache.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="entries"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="maximumStorageCapacity"/> is equal to zero.</exception>
        public IEnumerable<ICacheEntry<TKey>> GetExpiredEntries(IEnumerable<ICacheEntry<TKey>> entries, ulong maximumStorageCapacity)
        {
            if (entries == null)
                throw new ArgumentNullException(nameof(entries));
            if (maximumStorageCapacity == 0)
                throw new ArgumentOutOfRangeException(nameof(maximumStorageCapacity), "The maximum storage capacity must be non-zero.");

            ulong totalSum = 0;
            var validKeys = entries
                .OrderBy(e => e.CreationTime)
                .TakeWhile(e =>
                {
                    totalSum += e.Size;
                    return totalSum <= maximumStorageCapacity;
                })
                .Select(e => e.Key)
                .ToList();

            var validKeySet = new HashSet<TKey>(validKeys, KeyComparer);
            return entries.Where(e => !validKeySet.Contains(e.Key)).ToList();
        }
    }
}