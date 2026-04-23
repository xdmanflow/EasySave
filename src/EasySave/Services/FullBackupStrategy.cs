using System;
using System.Diagnostics;
using System.IO;
using EasySave.Models;
using EasyLog;

namespace EasySave.Services
{
    public class FullBackupStrategy : IBackupStrategy
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

            var files = Directory.GetFiles(job.SourceDirectory, "*", SearchOption.AllDirectories);
            long totalSize = 0;
            foreach (var f in files) totalSize += new FileInfo(f).Length;

            int remaining = files.Length;
            long copied = 0;

            stateLogger.Update(new StateEntry
            {
                Name = job.Name,
                State = "ACTIVE",
                TotalFilesToCopy = files.Length,
                TotalFilesSize = totalSize,
                NbFilesLeftToDo = remaining,
                Progression = 0
            });

            foreach (string src in files)
            {
                string rel = Path.GetRelativePath(job.SourceDirectory, src);
                string dst = Path.Combine(job.TargetDirectory, rel);
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

                long size = new FileInfo(src).Length;
                var sw = Stopwatch.StartNew();
                double ms;

                try
                {
                    File.Copy(src, dst, overwrite: true);
                    ms = sw.Elapsed.TotalMilliseconds;
                }
                catch
                {
                    ms = -sw.Elapsed.TotalMilliseconds;
                }

                dailyLogger.Log(new LogEntry
                {
                    Name = job.Name,
                    FileSource = ToUncPath(src),
                    FileTarget = ToUncPath(dst),
                    FileSize = size,
                    FileTransferTime = ms
                });

                copied += size;
                remaining--;

                stateLogger.Update(new StateEntry
                {
                    Name = job.Name,
                    State = "ACTIVE",
                    SourceFilePath = ToUncPath(src),
                    TargetFilePath = ToUncPath(dst),
                    TotalFilesToCopy = files.Length,
                    TotalFilesSize = totalSize,
                    NbFilesLeftToDo = remaining,
                    Progression = totalSize > 0 ? (int)(copied * 100 / totalSize) : 100
                });
            }

            stateLogger.SetEnd(job.Name);
        }
    }
}
