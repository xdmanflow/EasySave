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

        public void Execute(BackupJob job, DailyLogger dailyLogger, StateLogger stateLogger)
        {
            if (!Directory.Exists(job.SourceDirectory))
                throw new DirectoryNotFoundException($"Source not found: {job.SourceDirectory}");

            var allFiles = Directory.GetFiles(job.SourceDirectory, "*", SearchOption.AllDirectories);
            var filesToCopy = SelectFiles(allFiles, job.SourceDirectory, job.TargetDirectory);

            long totalSize = filesToCopy.Sum(f => new FileInfo(f).Length);
            int remaining = filesToCopy.Length;
            long bytesDone = 0;

            stateLogger.Update(new StateEntry
            {
                Name = job.Name,
                State = "ACTIVE",
                TotalFilesToCopy = filesToCopy.Length,
                TotalFilesSize = totalSize,
                NbFilesLeftToDo = remaining,
                Progression = 0
            });

            foreach (string source in filesToCopy)
            {
                string relative = Path.GetRelativePath(job.SourceDirectory, source);
                string target = Path.Combine(job.TargetDirectory, relative);
                Directory.CreateDirectory(Path.GetDirectoryName(target)!);

                long size = new FileInfo(source).Length;
                double elapsed = Transfer(source, target);

                dailyLogger.Log(new LogEntry
                {
                    Name = job.Name,
                    FileSource = source,
                    FileTarget = target,
                    FileSize = size,
                    FileTransferTime = elapsed
                });

                bytesDone += size;
                remaining--;

                stateLogger.Update(new StateEntry
                {
                    Name = job.Name,
                    State = "ACTIVE",
                    SourceFilePath = source,
                    TargetFilePath = target,
                    TotalFilesToCopy = filesToCopy.Length,
                    TotalFilesSize = totalSize,
                    NbFilesLeftToDo = remaining,
                    Progression = totalSize > 0 ? (int)(bytesDone * 100 / totalSize) : 100
                });
            }

            stateLogger.SetEnd(job.Name);
        }

        private double Transfer(string source, string target)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                File.Copy(source, target, overwrite: true);
                return sw.Elapsed.TotalMilliseconds;
            }
            catch (Exception)
            {
                return -sw.Elapsed.TotalMilliseconds;
            }
        }
    }
}
