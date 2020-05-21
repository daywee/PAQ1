using System;
using System.IO;

namespace Paq1.ConsoleApp
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Invalid arguments.");
                Console.WriteLine("Use c|d for compression and decompression then specify source and target paths.");
                Console.WriteLine("For example 'c original.txt compressed.paq'\n");
                return 0;
            }

            string mode = args[0];
            string sourcePath = args[1];
            string targetPath = args[2];


            if (mode != "c" && mode != "d")
            {
                foreach (var s in args)
                {
                    Console.WriteLine(s);
                }
                Console.WriteLine("Use c|d for compression and decompression.");
                return 0;
            }

            if (!File.Exists(sourcePath))
            {
                Console.WriteLine($"File {sourcePath} does not exist.");
                return 0;
            }

            var paq = new Core.Paq1();
            if (mode == "c")
            {
                Console.WriteLine("Compressing...");
                paq.Compress(sourcePath, targetPath);
            }
            else
            {
                Console.WriteLine("Decompressing...");
                paq.Decompress(sourcePath, targetPath);
            }

            Console.WriteLine("Finished.");
            return 1;
        }
    }
}
