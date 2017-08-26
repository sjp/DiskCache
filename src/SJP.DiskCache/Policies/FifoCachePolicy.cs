using System;
using System.Collections.Generic;
using System.Linq;

namespace SJP.DiskCache
{
    public class FifoCachePolicy : ICachePolicy
    {
        public IEnumerable<ICacheEntry> GetExpiredEntries(IEnumerable<ICacheEntry> entries, ulong maximumStorageCapacity)
        {
            if (entries == null)
                throw new ArgumentNullException(nameof(entries));

            ulong totalSum = 0;
            var validKeys = entries
                .OrderByDescending(e => e.CreationTime)
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