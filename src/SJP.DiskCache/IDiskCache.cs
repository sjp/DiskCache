using System;
using System.IO;
using System.Threading.Tasks;

namespace SJP.DiskCache
{
    // TODO split up into ICache and IDiskCache?
    //      not done yet because we're really only interested in disk caching
    //      maybe later something more generic?
    public interface IDiskCache : IDisposable
    {
        ulong MaximumStorageCapacity { get; }

        // TODO remove and place only on the DiskCache class itself
        //      as it's an implementation detail, not something for the interface
        TimeSpan PollingInterval { get; }

        ICachePolicy Policy { get; }

        event EventHandler<ICacheEntry> EntryAdded;

        event EventHandler<ICacheEntry> EntryUpdated;

        event EventHandler<ICacheEntry> EntryRemoved;

        void Clear();

        bool ContainsKey(string key);

        Task<bool> ContainsKeyAsync(string key);

        Stream GetValue(string key);

        bool TryGetValue(string key, out Stream stream);

        (bool hasValue, Stream stream) TryGetValue(string key);

        Task<Stream> GetValueAsync(string key);

        Task<(bool hasValue, Stream stream)> TryGetValueAsync(string key);

        void SetValue(string key, Stream value);

        bool TrySetValue(string key, Stream value);
    }
}