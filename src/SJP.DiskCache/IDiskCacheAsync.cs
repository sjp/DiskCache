using System;
using System.IO;
using System.Threading.Tasks;

namespace SJP.DiskCache
{
    /// <summary>
    /// Defines methods, properties and events used to manage an asynchronous disk-based cache.
    /// </summary>
    /// <typeparam name="TKey">The type of keys used in the cache.</typeparam>
    public interface IDiskCacheAsync<TKey> : IDisposable
    {
        /// <summary>
        /// The maximum size that the cache can contain.
        /// </summary>
        ulong MaximumStorageCapacity { get; }

        /// <summary>
        /// The cache eviction policy that evaluates which entries should be removed from the cache.
        /// </summary>
        ICachePolicy<TKey> Policy { get; }

        /// <summary>
        /// Occurs when an entry has been added to the cache.
        /// </summary>
        event EventHandler<ICacheEntry<TKey>> EntryAdded;

        /// <summary>
        /// Occurs when an entry has been updated in the cache.
        /// </summary>
        event EventHandler<ICacheEntry<TKey>> EntryUpdated;

        /// <summary>
        /// Occurs when an entry has been removed or evicted from the cache.
        /// </summary>
        event EventHandler<ICacheEntry<TKey>> EntryRemoved;

        /// <summary>
        /// Empties the cache of all values that it is currently tracking.
        /// </summary>
        Task ClearAsync();

        /// <summary>
        /// Asynchronously determines whether the <see cref="IDiskCache{T}"/> contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the cache.</param>
        /// <returns><c>true</c> if the cache contains the key; otherwise <c>false</c>.</returns>
        Task<bool> ContainsKeyAsync(TKey key);

        /// <summary>
        /// Asynchronously gets the value associated with a key.
        /// </summary>
        /// <param name="key">The key to locate in the cache.</param>
        /// <returns>A stream of data from the cache.</returns>
        Task<Stream> GetValueAsync(TKey key);

        /// <summary>
        /// Asynchronously gets the value associated with a key.
        /// </summary>
        /// <param name="key">The key to locate in the cache.</param>
        /// <returns>A tuple of two values. A boolean determines whether <paramref name="key"/> is present in the cache. If <paramref name="key"/> is present, the <see cref="Stream"/> value will be provided, otherwise it will be <c>null</c>.</returns>
        Task<(bool hasValue, Stream stream)> TryGetValueAsync(TKey key);

        /// <summary>
        /// Asynchronously stores a value associated with a key.
        /// </summary>
        /// <param name="key">The key used to locate the value in the cache.</param>
        /// <param name="value">A stream of data to store in the cache.</param>
        Task SetValueAsync(TKey key, Stream value);

        /// <summary>
        /// Asynchronously stores a value associated with a key.
        /// </summary>
        /// <param name="key">The key used to locate the value in the cache.</param>
        /// <param name="value">A stream of data to store in the cache.</param>
        /// <returns><c>true</c> if the data was able to be stored without error; otherwise <c>false</c>.</returns>
        Task<bool> TrySetValueAsync(TKey key, Stream value);
    }
}