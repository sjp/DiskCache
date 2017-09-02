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

**TODO**

## API

**TODO**

## Icon

The project icon was created by myself using a combination of two images, in addition to modifying these two images. The hard drive icon was created by [Revicon](https://www.flaticon.com/authors/revicon), while the lightning icon was created by [Maxim Basinski](https://www.flaticon.com/authors/maxim-basinski).