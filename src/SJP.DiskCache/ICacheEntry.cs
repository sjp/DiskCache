using System;

namespace SJP.DiskCache
{
    public interface ICacheEntry
    {
        string Key { get; }

        ulong Size { get; }

        DateTime LastAccessed { get; }

        DateTime CreationTime { get; }

        ulong AccessCount { get; }

        // updates information about the cache entry
        // mostly updates last accessed and access count
        void Refresh();
    }
}