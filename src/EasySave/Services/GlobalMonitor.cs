using System.Threading;

namespace EasySave.Services
{
    public static class GlobalMonitor
    {
        private static int _priorityFilesCount = 0;
        private static readonly object _priorityLock = new object();
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
            if (isPriorityFile) return;
            lock (_priorityLock)
            {
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
                _largeFileSemaphore.Wait();
        }

        public static void ReleaseIfLargeFile(long fileSize, long maxFileSizeKB)
        {
            if (maxFileSizeKB <= 0) return;
            long maxFileSizeBytes = maxFileSizeKB * 1024;
            if (fileSize > maxFileSizeBytes)
                _largeFileSemaphore.Release();
        }
    }
}