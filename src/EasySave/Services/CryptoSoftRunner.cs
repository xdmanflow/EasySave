using System.Diagnostics;

namespace EasySave.Services
{
    public static class CryptoSoftRunner
    {
        private static readonly object _encryptLock = new object();

        public static double Encrypt(string cryptoSoftPath, string filePath)
        {
            if (string.IsNullOrWhiteSpace(cryptoSoftPath)) return 0;

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

                    if (proc?.ExitCode == -4)
                        return -1;

                    return proc?.ExitCode >= 0
                        ? sw.Elapsed.TotalMilliseconds
                        : -sw.Elapsed.TotalMilliseconds;
                }
                catch { return -1; }
            }
        }
    }
}