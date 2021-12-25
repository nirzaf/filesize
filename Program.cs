using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace downloader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var options = FileOptions.None;
            string fileName = null;
            var bufferSize = 4096;
            foreach (var arg in args)
            {
                switch (arg)
                {
                    case "-nobuff": options |= (FileOptions)0x20000000; break;
                    case "-async": options |= FileOptions.Asynchronous; break;
                    case var a when arg.StartsWith("-buff="): bufferSize = int.Parse(a.Substring("-buff=".Length)); break;
                    default: fileName = arg; break;
                }
            }

            if (fileName == null)
            {
                Console.WriteLine("Usage: filesize [option] <path>");
                return;
            }

            using var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, options);

            var buffer = new byte[4096];
            int size = 0, read = 0;

            var sw = Stopwatch.StartNew();
            do
            {
                read = await stream.ReadAsync(buffer, 0, buffer.Length);
                size += read;
            }
            while (read != 0);

            sw.Stop();

            Console.WriteLine($"File {fileName} size is {size}' ({sw.Elapsed.TotalMilliseconds:#,#} ms)");
        }
    }
}
