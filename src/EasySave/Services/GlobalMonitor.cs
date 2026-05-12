using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace EasySave.Services
{
    public static class GlobalMonitor
    {
        private static int _priorityFilesCount = 0;
        private static readonly object _priorityLock = new object();

        // Semaphore allows only 1 large file to pass through at a time
        private static readonly SemaphoreSlim _largeFileSemaphore = new SemaphoreSlim(1, 1);

        public static void RegisterPriorityFiles(int count)
        {
            if (count <= 0) return;
            lock (_priorityLock)
            {
                _priorityFilesCount += count;
            }
        }

        public static void RemovePriorityFiles(int count)
        {
            if (count <= 0) return;
            lock (_priorityLock)
            {
                _priorityFilesCount -= count;
                // If no more priority files exist globally, wake up all waiting non-priority threads
                if (_priorityFilesCount <= 0)
                {
                    _priorityFilesCount = 0;
                    Monitor.PulseAll(_priorityLock);
                }
            }
        }

        public static void DecrementPriorityFile()
        {
            RemovePriorityFiles(1);
        }

        public static void WaitIfNonPriority(bool isPriorityFile)
        {
            if (isPriorityFile) return; // Priority files pass freely

            lock (_priorityLock)
            {
                // Non-priority files must wait here as long as the global count is > 0
                while (_priorityFilesCount > 0)
                {
                    Monitor.Wait(_priorityLock);
                }
            }
        }

        public static void WaitIfLargeFile(long fileSize, long maxFileSizeKB)
        {
            if (maxFileSizeKB <= 0) return;

            long maxFileSizeBytes = maxFileSizeKB * 1024;
            if (fileSize > maxFileSizeBytes)
            {
                _largeFileSemaphore.Wait(); // Locks if another large file is already transferring
            }
        }

        public static void ReleaseIfLargeFile(long fileSize, long maxFileSizeKB)
        {
            if (maxFileSizeKB <= 0) return;

            long maxFileSizeBytes = maxFileSizeKB * 1024;
            if (fileSize > maxFileSizeBytes)
            {
                _largeFileSemaphore.Release(); // Unlocks for the next large file
            }
        }
    }
}
