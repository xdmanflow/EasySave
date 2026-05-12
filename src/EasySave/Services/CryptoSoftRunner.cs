using System;
using System.Diagnostics;

namespace EasySave.Services
{
    public static class CryptoSoftRunner
    {
        // --- NEW: Lock object to synchronize parallel threads ---
        private static readonly object _encryptLock = new object();

        public static double Encrypt(string cryptoSoftPath, string filePath)
        {
            if (string.IsNullOrWhiteSpace(cryptoSoftPath)) return 0;

            // --- NEW: Only one thread can enter this block at a time ---
            lock (_encryptLock)
            {
                try
                {
                    var sw = Stopwatch.StartNew();
                    var psi = new ProcessStartInfo
                    {
                        FileName = cryptoSoftPath,
                        Arguments = $"\"{filePath}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    using var proc = Process.Start(psi);
                    proc?.WaitForExit();
                    sw.Stop();

                    // If CryptoSoft returns -4, it means it was rejected by the Mutex
                    if (proc?.ExitCode == -4)
                    {
                        return -1;
                    }

                    return proc?.ExitCode >= 0
                        ? sw.Elapsed.TotalMilliseconds
                        : -sw.Elapsed.TotalMilliseconds;
                }
                catch { return -1; }
            }
        }
    }
}