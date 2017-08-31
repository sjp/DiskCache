using System;
using System.Collections.Generic;
using System.Linq;

namespace SJP.DiskCache
{
    /// <summary>
    /// Evicts values in a cache that have been stored for a given time period.
    /// </summary>
    public class FixedTimespanCachePolicy : ICachePolicy
    {
        /// <summary>
        /// Initializes a fixed timespan cache policy.
        /// </summary>
        /// <param name="timeSpan">The timespan that a value should be kept in the cache for.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeSpan"/> is a negative or zero-length timespan.</exception>
        public FixedTimespanCachePolicy(TimeSpan timeSpan)
        {
            if (timeSpan <= _zero)
                throw new ArgumentOutOfRangeException("Expiration time spans must be non-negative and non-zero. The given timespan was instead " + timeSpan.ToString(), nameof(timeSpan));

            ExpirationTimespan = timeSpan;
        }

        /// <summary>
        /// The maximum length of time that a value is allowed to be kept in the cache.
        /// </summary>
        public TimeSpan ExpirationTimespan { get; }

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

            var currentTime = DateTime.Now;
            ulong totalSum = 0;
            var validKeys = entries
                .OrderByDescending(e => e.CreationTime) // for the valid objects keep the newest
                .TakeWhile(e =>
                {
                    var hasExpired = (currentTime - e.CreationTime) > ExpirationTimespan;
                    if (hasExpired)
                        return false;
                    totalSum += e.Size;
                    return totalSum <= maximumStorageCapacity;
                })
                .Select(e => e.Key)
                .ToList();

            var validKeySet = new HashSet<string>(validKeys);
            return entries.Where(e => !validKeySet.Contains(e.Key)).ToList();
        }

        private readonly static TimeSpan _zero = new TimeSpan(0);
    }
}