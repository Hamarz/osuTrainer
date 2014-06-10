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
            string progExe = args[0];
            string updateUrl = args[1];
            var progName = Path.GetFileNameWithoutExtension(progExe);

            Process[] process = Process.GetProcessesByName(progName);
            if (process.Length > 0)
            {
                Console.WriteLine("Closing " + progName);
                process[0].Kill();
                if (process[0] != null)
                {
                    process[0].WaitForExit();
                }
            }

            WebRequest.DefaultWebProxy = new WebProxy();
            using (var client = new WebClient())
            {
                Console.WriteLine("Downloading " + updateUrl);
                client.DownloadFile(updateUrl, "temp");
            }

            using (ZipArchive zip = ZipFile.OpenRead("temp"))
            {
                foreach (ZipArchiveEntry file in zip.Entries)
                {
                    if (file.FullName == "Updater.exe") continue;
                    Console.WriteLine("Extracting " + Path.Combine(Directory.GetCurrentDirectory(), file.FullName));
                    file.ExtractToFile(Path.Combine(Directory.GetCurrentDirectory(), file.FullName), true);
                }               
            }
            File.Delete("temp");
            Console.WriteLine("Update complete.");
            Process.Start(progExe);
        }
    }
}