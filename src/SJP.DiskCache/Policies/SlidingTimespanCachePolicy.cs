using System;
using System.Collections.Generic;
using System.Linq;

namespace SJP.DiskCache
{
    public class SlidingTimespanCachePolicy : ICachePolicy
    {
        public SlidingTimespanCachePolicy(TimeSpan timeSpan)
        {
            if (timeSpan < _zero)
                throw new ArgumentOutOfRangeException("Expiration time spans must be non-negative. The given timespan was instead " + timeSpan.ToString(), nameof(timeSpan));

            ExpirationTimespan = timeSpan;
        }

        public TimeSpan ExpirationTimespan { get; }

        public IEnumerable<ICacheEntry> GetExpiredEntries(IEnumerable<ICacheEntry> entries, ulong maximumStorageCapacity)
        {
            if (entries == null)
                throw new ArgumentNullException(nameof(entries));

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

            var validKeySet = new HashSet<string>(validKeys);
            return entries.Where(e => !validKeySet.Contains(e.Key)).ToList();
        }

        private readonly static TimeSpan _zero = new TimeSpan(0);
    }
}