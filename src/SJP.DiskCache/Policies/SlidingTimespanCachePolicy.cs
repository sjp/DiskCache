using System;
using System.Collections.Generic;
using System.Linq;

namespace SJP.DiskCache
{
    /// <summary>
    /// Evicts values in a cache that have been not been accessed for a given time period.
    /// </summary>
    /// <typeparam name="TKey">The type of keys used in the cache.</typeparam>
    public class SlidingTimespanCachePolicy<TKey> : ICachePolicy<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Initializes a sliding timespan cache policy.
        /// </summary>
        /// <param name="timeSpan">The timespan that a value should be kept in the cache for.</param>
        /// <param name="keyComparer">The <see cref="IEqualityComparer{TKey}"/> implementation to use when comparing cache keys, or <c>null</c> to use the default <see cref="EqualityComparer{TKey}"/> implementation for the set type.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeSpan"/> is a negative or zero-length timespan.</exception>
        public SlidingTimespanCachePolicy(TimeSpan timeSpan, IEqualityComparer<TKey> keyComparer = null)
        {
            if (timeSpan <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("Expiration time spans must be non-negative and non-zero. The given timespan was instead " + timeSpan.ToString(), nameof(timeSpan));

            ExpirationTimespan = timeSpan;
            KeyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
        }

        /// <summary>
        /// The maximum length of time that a value is allowed to be kept in the cache without being accessed.
        /// </summary>
        public TimeSpan ExpirationTimespan { get; }

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

            var currentTime = DateTime.Now;
            ulong totalSum = 0;
            var validKeys = entries
                .OrderByDescending(e => e.LastAccessed) // for the valid objects, keep the newest
                .TakeWhile(e =>
                {
                    var hasExpired = (currentTime - e.LastAccessed) > ExpirationTimespan;
                    if (hasExpired)
                        return false;
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