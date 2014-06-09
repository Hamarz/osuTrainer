using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace Updater
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 2) Environment.Exit(1);
            string progName = args[0];
            string updateUrl = args[1];

            Process[] process = Process.GetProcessesByName(progName);
            if (process.Length > 0)
            {
                Console.WriteLine("Closing " + progName);
                process[0].Close();
                process[0].WaitForExit();
            }

            WebRequest.DefaultWebProxy = new WebProxy();
            using (var client = new WebClient())
            {
                Console.WriteLine("Downloading " + updateUrl);
                client.DownloadFile(updateUrl, "temp");
            }
            ZipArchive zip = ZipFile.OpenRead("temp");
            foreach (ZipArchiveEntry file in zip.Entries)
            {
                Console.WriteLine("Extracting " + Path.Combine(Directory.GetCurrentDirectory(), file.FullName));
                file.ExtractToFile(Path.Combine(Directory.GetCurrentDirectory(), file.FullName), true);
            }
            Console.WriteLine("Update complete.");
            File.Delete("temp");
            Process.Start(args[0]);
        }
    }
}