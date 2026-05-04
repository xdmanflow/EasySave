using System;
using System.Diagnostics;
using System.IO;

namespace CryptoSoft
{
    internal class Program
    {
        private static readonly byte[] Key = { 0x4A, 0x6F, 0x79, 0x21 };

        static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("Usage: CryptoSoft.exe <file_path>");
                return -1;
            }

            string filePath = args[0];

            if (!File.Exists(filePath))
            {
                Console.Error.WriteLine($"File not found: {filePath}");
                return -2;
            }

            try
            {
                var sw = Stopwatch.StartNew();

                byte[] data = File.ReadAllBytes(filePath);

                for (int i = 0; i < data.Length; i++)
                    data[i] ^= Key[i % Key.Length];

                File.WriteAllBytes(filePath, data);

                sw.Stop();

                Console.WriteLine($"Encrypted in {sw.ElapsedMilliseconds} ms");
                return (int)sw.ElapsedMilliseconds;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                return -3;
            }
        }
    }
}