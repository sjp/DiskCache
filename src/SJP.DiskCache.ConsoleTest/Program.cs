using System;
using System.IO;

namespace SJP.DiskCache.ConsoleTest
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var cacheDir = new DirectoryInfo(@"C:\Users\sjp\Downloads\tmp\cache");
            var cachePolicy = new LruCachePolicy();
            const ulong maxSize = 1024L * 1024L * 1024L * 1024L; // 1GB
            using (var diskCache = new DiskCache(cacheDir, cachePolicy, maxSize))
            {
                diskCache.EntryAdded += (s, e) => Console.WriteLine($"Added: { e.Key }, { e.Size }");

                using (var flacStream = File.OpenRead(@"C:\Users\sjp\Downloads\05. End Of Days.flac"))
                    diskCache.SetValue("flacFile", flacStream);

                var containsTest = diskCache.ContainsKey("flacFile");
                if (containsTest)
                {
                    using (var outStr = diskCache.GetValue("flacFile"))
                    using (var tmpFile = File.OpenWrite(@"C:\Users\sjp\Downloads\tmp.flac"))
                        outStr.CopyTo(tmpFile);
                }

                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(true);
            }
        }
    }
}
