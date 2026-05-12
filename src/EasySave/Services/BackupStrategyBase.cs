using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading; 
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

            string[] all = Directory.GetFiles(job.SourceDirectory, "*", SearchOption.AllDirectories);
            string[] files = SelectFiles(all, job.SourceDirectory, job.TargetDirectory);

            long totalSize = files.Sum(f => new FileInfo(f).Length);
            int remaining = files.Length;
            long done = 0;

            job.TotalFilesSize = totalSize;
            job.TotalSizeCopied = 0;
            job.Progress = 0.0;

            // --- PRE-CALCULATE PRIORITY FILES FOR THIS JOB ---
            int priorityCount = files.Count(f => settings.PriorityExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()));
            int remainingPriorityFiles = priorityCount;
            GlobalMonitor.RegisterPriorityFiles(priorityCount);

            try
            {
                stateLogger.Update(new StateEntry
                {
                    Name = job.Name,
                    State = "ACTIVE",
                    TotalFilesToCopy = files.Length,
                    TotalFilesSize = totalSize,
                    NbFilesLeftToDo = remaining,
                    Progression = 0
                });

                foreach (string source in files)
                {
                    // 1. Check for manual pause
                    job.PauseEvent.Wait();

                    // 2. Check for manual stop
                    if (job.State == JobState.Stopped)
                    {
                        LogStop(dailyLogger, job.Name);
                        break;
                    }

                    // --- NEW FEATURE 5: TEMPORARY PAUSE FOR BUSINESS SOFTWARE ---
                    bool isWaitingForSoftware = false;
                    while (BusinessSoftwareDetector.IsRunning(settings.BusinessSoftware))
                    {
                        if (!isWaitingForSoftware)
                        {
                            isWaitingForSoftware = true;
                            // Optionally update state to show it's waiting
                            stateLogger.Update(new StateEntry
                            {
                                Name = job.Name,
                                State = "WAITING_BUSINESS_SOFT",
                                SourceFilePath = source,
                                Progression = (int)job.Progress
                            });
                        }

                        Thread.Sleep(2000); // Sleep for 2 seconds then check again

                        // If user clicks Stop while we are waiting, break out of the loop
                        if (job.State == JobState.Stopped) break;
                    }

                    // If we broke out of the loop because of a manual stop, exit the transfer loop
                    if (job.State == JobState.Stopped)
                    {
                        LogStop(dailyLogger, job.Name);
                        break;
                    }

                    // If we were waiting, set state back to ACTIVE
                    if (isWaitingForSoftware)
                    {
                        stateLogger.Update(new StateEntry
                        {
                            Name = job.Name,
                            State = "ACTIVE",
                            SourceFilePath = source,
                            Progression = (int)job.Progress
                        });
                    }
                    // -------------------------------------------------------------

                    string relative = Path.GetRelativePath(job.SourceDirectory, source);
                    string target = Path.Combine(job.TargetDirectory, relative);
                    Directory.CreateDirectory(Path.GetDirectoryName(target)!);

                    long size = new FileInfo(source).Length;
                    string ext = Path.GetExtension(source).ToLowerInvariant();
                    bool isPriority = settings.PriorityExtensions.Contains(ext);

                    // --- WAIT FOR RULES ---
                    GlobalMonitor.WaitIfNonPriority(isPriority);
                    GlobalMonitor.WaitIfLargeFile(size, settings.MaxFileSizeKB);

                    double elapsed = 0;
                    try
                    {
                        elapsed = Transfer(source, target);
                    }
                    finally
                    {
                        // --- RELEASE RULES ---
                        GlobalMonitor.ReleaseIfLargeFile(size, settings.MaxFileSizeKB);
                        if (isPriority)
                        {
                            GlobalMonitor.DecrementPriorityFile();
                            remainingPriorityFiles--;
                        }
                    }

                    double encTime = 0;
                    if (settings.EncryptedExtensions.Contains(ext))
                        encTime = CryptoSoftRunner.Encrypt(settings.CryptoSoftPath, target);

                    dailyLogger.Log(new LogEntry
                    {
                        Name = job.Name,
                        FileSource = source,
                        FileTarget = target,
                        FileSize = size,
                        FileTransferTime = elapsed,
                        EncryptionTime = encTime
                    });

                    done += size;
                    remaining--;
                    job.TotalSizeCopied = done;
                    job.Progress = totalSize > 0 ? Math.Round(((double)done / totalSize) * 100.0, 2) : 100.0;

                    stateLogger.Update(new StateEntry
                    {
                        Name = job.Name,
                        State = "ACTIVE",
                        SourceFilePath = source,
                        TargetFilePath = target,
                        TotalFilesToCopy = files.Length,
                        TotalFilesSize = totalSize,
                        NbFilesLeftToDo = remaining,
                        Progression = (int)job.Progress
                    });
                }

                if (job.State != JobState.Stopped && job.State != JobState.Error)
                {
                    stateLogger.SetEnd(job.Name);
                    job.State = JobState.Completed;
                }
            }
            finally
            {
                // CRITICAL: If the job crashes or is stopped, remove remaining priority files from the global count
                if (remainingPriorityFiles > 0)
                {
                    GlobalMonitor.RemovePriorityFiles(remainingPriorityFiles);
                }
            }
        }

        private static void LogStop(DailyLogger logger, string jobName)
        {
            logger.Log(new LogEntry
            {
                Name = jobName,
                FileSource = "[STOPPED BY USER]",
                FileTarget = string.Empty,
                FileSize = 0,
                FileTransferTime = 0,
                EncryptionTime = 0
            });
        }

        private static double Transfer(string source, string target)
        {
            var sw = Stopwatch.StartNew();
            try { File.Copy(source, target, overwrite: true); return sw.Elapsed.TotalMilliseconds; }
            catch { return -sw.Elapsed.TotalMilliseconds; }
        }
    }
}