<h1 align="center">
	<br>
	<img width="256" height="256" src="diskcache.png" alt="DiskCache">
	<br>
	<br>
</h1>

> Disk-based caching for large objects in .NET.

[![License (MIT)](https://img.shields.io/badge/license-MIT-blue.svg)](https://opensource.org/licenses/MIT) [![Build status](https://ci.appveyor.com/api/projects/status/x7di32g4cb8oriye?svg=true)](https://ci.appveyor.com/project/sjp/diskcache)

Enables caching of large objects in memory-constrained environments. Data can be temporarily stored on disk using a variety of cache policies. An example use case is to temporarily store transcoded audio or video files to avoid re-encoding the source files.

## Highlights

* Supports .NET Framework 4.5+, and .NET Standard 2.0+.
* Streams input and output to limit memory usage.
* Generic cache keys supported.
* Configurable cache policies. Examples include LRU, FIFO, MRU.

## Installation

**NOTE:** This project is still a work in progress. However, once ready, it will be available by the following methods.

```powershell
Install-Package SJP.DiskCache
```

or

```console
dotnet add package SJP.DiskCache
```

## Usage

A simple demonstration of `DiskCache` is to use it as a cache for storing at-most 1GB of data. The purpose of this example cache is to store `MP3` files that have been temporarily transcoded from the `FLAC` format. How we'll be achieving that is by using the path to the `FLAC` file as a key, to point to the temporary `MP3` file.

```csharp
var cacheDir = new DirectoryInfo(@"C:\tmp\cache");   // where to store the cache
var cachePolicy = new LruCachePolicy<string>();      // using an LRU cache policy
const ulong maxSize = 1024L * 1024L * 1024L * 1024L; // 1GB
```

The cache policy can be any type that implements `IEquatable<T>`, however, in our case it's a `string` because we want to use paths to `FLAC` files as a key. The particular cache policy we are using is an LRU policy, meaning that when the cache is required to evict objects, it will remove least recently used objects. This means that frequently accessed objects will remain in the cache.

```csharp
using (var diskCache = new DiskCache<string>(cacheDir, cachePolicy, maxSize))
{
    // store values
    foreach (string flacFilePath in Directory.EnumerateFiles(@"C:\tmp\flac", "*.flac"))
    {
        // convert to MP3 and store in the cache
        using (Stream mp3Stream = FlacConvert.ToMp3(flacFilePath))
            diskCache.SetValue(flacFilePath, mp3Stream);
    }

    // now get an example value, will be (almost) immediate
    var mp3FileStream = diskCache.GetValue(@"C:\tmp\flac\example.flac");
}
```

The above code listing shows quite a lot going on. Put simply, we find all `FLAC` files, convert them to `MP3` and store the result in the cache. Later we retrieve the value from the cache at no cost (at least when compared to transcoding). While this is not as performant and is more I/O heavy than other approaches, the major advantage it has is that memory-constrained environments (e.g. Raspberry Pis) are able to use this cache rather than very quickly exhausting memory just to store temporary MP3 files.

## API

### `DiskCache<T>`

The `DiskCache<T>` object takes a type parameter of any type that implements `IEquatable<T>`. This is so that the keys of the cache can be of any type, so long as they can be distinct.

To construct a `DiskCache<T>` requires a cache directory, a maximum storage size for that directory (i.e. a quota), and a cache policy (discussed later).

```csharp
var cacheDir = new DirectoryInfo(@"C:\tmp\cache");   // where to store the cache
var cachePolicy = new LruCachePolicy<string>();      // using an LRU cache policy
const ulong maxSize = 1024L * 1024L * 1024L * 1024L; // 1GB
```

We can now create the `DiskCache<T>` object.

```csharp
var diskCache = new DiskCache<string>(cacheDir, cachePolicy, maxSize);
```

Given a `DiskCache<T>` object, and the fact that it is designed to be a key-value store, there are only really two operations that can be applied, get and set.

#### `SetValue(T key, Stream value)`

Values can be added and updated in the cache using `SetValue()`. The arguments are the key to use for the cache entry, and a stream to serialize to disk. There are other variations, `SetValueAsync()`, `TrySetValue()`, `TrySetValueAsync()`, which will not be further described here.

The following example shows how an `MP3` file when temporarily converted from a `FLAC` file can be stored in the cache.

```csharp
var flacFilePath = @"C:\tmp\flac\example.flac";
using (Stream mp3Stream = FlacConvert.ToMp3(flacFilePath))
    diskCache.SetValue(flacFilePath, mp3Stream);
```

#### `GetValue(T key)`

We can retrieve a value from the cache using `GetValue()`. Given a key, it will return a stream from the file on disk. There are other variations, `GetValueAsync()`, `TryGetValue()`, `TryGetValueAsync()`, which will not be further described here.

```csharp
var flacFilePath = @"C:\tmp\flac\example.flac";
Stream mp3FileStream = diskCache.GetValue(@"C:\tmp\flac\example.flac");
```

The resulting stream will be an `MP3` stream that was transcoded from the `FLAC` file that is referred to in the `flacFilePath` variable.

### `ICachePolicy<T>`

The cache policies defined in `DiskCache` are required to implement `ICachePolicy<T>`. There are several implementations of popular cache policies, which will now be described.

* `FifoCachePolicy` -- A FIFO (first-in-first-out) cache policy. Values that were introduced to the cache earliest will be removed first.
* `FixedTimespanCachePolicy` -- A policy which always evicts values after a given time period. This is useful when you expect the values to no longer be useful after 1 hour (for example).
* `LfuCachePolicy` -- An LFU (least-frequently-used) cache policy. Entries that have not been accessed many times will be removed before more popular cache entries.
* `LifoCachePolicy` -- A LIFO (last-in-first-out) cache policy. Values that were added to the cache most recently will be removed first.
* `LruCachePolicy` -- An LRU (least-recently-used) cache policy. Entries that have not been accessed for a long period of time will be removed before more recently accessed entries.
* `MfuCachePolicy` -- An MFU (most-frequently-used) cache policy. Entries that have been accessed many times will be removed before less popular cache entries.
* `MruCachePolicy` -- An MRU (most-recently-used) cache policy. Entries that have been accessed for recently will be removed before less recently accessed entries.
* `SlidingTimespanCachePolicy` -- A policy which evicts values if they have not been accessed for a given time period. This can be used to implement a cache which removed entries that have not been accessed within 5 minutes.

## Icon

The project icon was created by myself using a combination of two images, in addition to modifying these two images. The hard drive icon was created by [Revicon](https://www.flaticon.com/authors/revicon), while the lightning icon was created by [Maxim Basinski](https://www.flaticon.com/authors/maxim-basinski).