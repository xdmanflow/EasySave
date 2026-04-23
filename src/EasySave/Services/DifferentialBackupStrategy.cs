using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using EasySave.Models;
using EasyLog;

namespace EasySave.Services
{
    public class DifferentialBackupStrategy : IBackupStrategy
    {
        private static string ToUncPath(string path)
        {
            string fullPath = Path.GetFullPath(path);
            if (fullPath.StartsWith(@"\\"))
                return fullPath;

            string root = Path.GetPathRoot(fullPath) ?? string.Empty;
            if (root.Length >= 3 && root[1] == ':')
            {
                char drive = char.ToUpperInvariant(root[0]);
                string relative = fullPath.Substring(root.Length)
                    .Replace(Path.DirectorySeparatorChar, '\\')
                    .Replace(Path.AltDirectorySeparatorChar, '\\');
                return $@"\\localhost\{drive}$\{relative}";
            }

            return fullPath;
        }

        public void Execute(BackupJob job, DailyLogger dailyLogger, StateLogger stateLogger)
        {
            if (!Directory.Exists(job.SourceDirectory))
                throw new DirectoryNotFoundException($"Source directory not found: {job.SourceDirectory}");

            var candidates = Directory.GetFiles(job.SourceDirectory, "*", SearchOption.AllDirectories);

            var queue = candidates.Where(src =>
            {
                string target = Path.Combine(job.TargetDirectory, Path.GetRelativePath(job.SourceDirectory, src));
                return !File.Exists(target) || File.GetLastWriteTimeUtc(src) > File.GetLastWriteTimeUtc(target);
            }).ToArray();

            long totalSize = queue.Sum(f => new FileInfo(f).Length);
            int left = queue.Length;
            long done = 0;

            stateLogger.Update(new StateEntry
            {
                Name = job.Name,
                State = "ACTIVE",
                TotalFilesToCopy = queue.Length,
                TotalFilesSize = totalSize,
                NbFilesLeftToDo = left,
                Progression = 0
            });

            foreach (string src in queue)
            {
                string rel    = Path.GetRelativePath(job.SourceDirectory, src);
                string dst    = Path.Combine(job.TargetDirectory, rel);
                long   fSize  = new FileInfo(src).Length;

                string dstDir = Path.GetDirectoryName(dst)!;
                bool dirExisted = Directory.Exists(dstDir);
                Directory.CreateDirectory(dstDir);
                if (!dirExisted)
                {
                    dailyLogger.Log(new LogEntry
                    {
                        Name = job.Name,
                        FileSource = ToUncPath(dstDir),
                        FileTarget = ToUncPath(dstDir),
                        FileSize = 0,
                        FileTransferTime = 0
                    });
                }

                var watch = Stopwatch.StartNew();
                double elapsed;

                try   { File.Copy(src, dst, true); elapsed =  watch.Elapsed.TotalMilliseconds; }
                catch { elapsed = -watch.Elapsed.TotalMilliseconds; }

                dailyLogger.Log(new LogEntry
                {
                    Name             = job.Name,
                    FileSource       = ToUncPath(src),
                    FileTarget       = ToUncPath(dst),
                    FileSize         = fSize,
                    FileTransferTime = elapsed
                });

                done += fSize;
                left--;

                stateLogger.Update(new StateEntry
                {
                    Name            = job.Name,
                    State           = "ACTIVE",
                    SourceFilePath  = ToUncPath(src),
                    TargetFilePath  = ToUncPath(dst),
                    TotalFilesToCopy = queue.Length,
                    TotalFilesSize  = totalSize,
                    NbFilesLeftToDo = left,
                    Progression     = totalSize > 0 ? (int)(done * 100 / totalSize) : 100
                });
            }

            stateLogger.SetEnd(job.Name);
        }
    }
}
