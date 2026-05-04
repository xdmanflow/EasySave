using System;
using System.Diagnostics;

namespace EasySave.Services
{
    public static class CryptoSoftRunner
    {
        public static double Encrypt(string cryptoSoftPath, string filePath)
        {
            if (string.IsNullOrWhiteSpace(cryptoSoftPath)) return 0;
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
                return proc?.ExitCode >= 0
                    ? sw.Elapsed.TotalMilliseconds
                    : -sw.Elapsed.TotalMilliseconds;
            }
            catch { return -1; }
        }
    }
}