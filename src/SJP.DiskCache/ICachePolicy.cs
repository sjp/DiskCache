using System.Collections.Generic;

namespace SJP.DiskCache
{
    public interface ICachePolicy
    {
        IEnumerable<ICacheEntry> GetExpiredEntries(IEnumerable<ICacheEntry> entries, ulong maximumStorageCapacity);
    }
}