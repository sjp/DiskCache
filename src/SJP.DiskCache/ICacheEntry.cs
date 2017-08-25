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
    }
}