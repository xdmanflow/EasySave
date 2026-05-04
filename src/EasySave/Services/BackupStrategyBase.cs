using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using EasySave.Models;
using EasyLog;

namespace EasySave.Services
{
    
    public abstract class BackupStrategyBase : IBackupStrategy
    {
        
        protected abstract string[] SelectFiles(string[] allFiles, string sourceDir, string targetDir);

        public void Execute(BackupJob job, DailyLogger dailyLogger, StateLogger stateLogger, AppSettings settings)
        {
            if (!Directory.Exists(job.SourceDirectory))
                throw new DirectoryNotFoundException($"Source not found: {job.SourceDirectory}");

            string[] all   = Directory.GetFiles(job.SourceDirectory, "*", SearchOption.AllDirectories);
            string[] files = SelectFiles(all, job.SourceDirectory, job.TargetDirectory);

            long totalSize = files.Sum(f => new FileInfo(f).Length);
            int remaining  = files.Length;
            long done      = 0;

            stateLogger.Update(new StateEntry
            {
                Name             = job.Name,
                State            = "ACTIVE",
                TotalFilesToCopy = files.Length,
                TotalFilesSize   = totalSize,
                NbFilesLeftToDo  = remaining,
                Progression      = 0
            });

            foreach (string source in files)
            {
                
                if (BusinessSoftwareDetector.IsRunning(settings.BusinessSoftware))
                {
                    dailyLogger.Log(new LogEntry
                    {
                        Name             = job.Name,
                        FileSource       = "[STOPPED: business software detected]",
                        FileTarget       = string.Empty,
                        FileSize         = 0,
                        FileTransferTime = -1,
                        EncryptionTime   = 0
                    });
                    stateLogger.SetEnd(job.Name);
                    throw new InvalidOperationException(
                        $"Job '{job.Name}' stopped: business software detected.");
                }

                string relative = Path.GetRelativePath(job.SourceDirectory, source);
                string target   = Path.Combine(job.TargetDirectory, relative);
                Directory.CreateDirectory(Path.GetDirectoryName(target)!);

                long   size    = new FileInfo(source).Length;
                double elapsed = Transfer(source, target);

                
                double encTime = 0;
                string ext     = Path.GetExtension(source).ToLowerInvariant();
                if (settings.EncryptedExtensions.Contains(ext))
                    encTime = CryptoSoftRunner.Encrypt(settings.CryptoSoftPath, target);

                dailyLogger.Log(new LogEntry
                {
                    Name             = job.Name,
                    FileSource       = source,
                    FileTarget       = target,
                    FileSize         = size,
                    FileTransferTime = elapsed,
                    EncryptionTime   = encTime
                });

                done += size;
                remaining--;

                stateLogger.Update(new StateEntry
                {
                    Name             = job.Name,
                    State            = "ACTIVE",
                    SourceFilePath   = source,
                    TargetFilePath   = target,
                    TotalFilesToCopy = files.Length,
                    TotalFilesSize   = totalSize,
                    NbFilesLeftToDo  = remaining,
                    Progression      = totalSize > 0 ? (int)(done * 100 / totalSize) : 100
                });
            }

            stateLogger.SetEnd(job.Name);
        }

        private static double Transfer(string source, string target)
        {
            var sw = Stopwatch.StartNew();
            try   { File.Copy(source, target, overwrite: true); return  sw.Elapsed.TotalMilliseconds; }
            catch { return -sw.Elapsed.TotalMilliseconds; }
        }
    }
}