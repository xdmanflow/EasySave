using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasySave.Languages;
using EasySave.Models;
using EasySave.Services;
using EasyLog;

namespace EasySave
{
    public class BackupManager
    {
        private readonly List<BackupJob> _jobs;
        private readonly DailyLogger _daily;
        private readonly StateLogger _state;
        private readonly LanguageManager _lang;
        private readonly AppSettings _settings;

        public BackupManager(
            List<BackupJob> jobs, DailyLogger daily,
            StateLogger state, LanguageManager lang, AppSettings settings)
        {
            _jobs = jobs;
            _daily = daily;
            _state = state;
            _lang = lang;
            _settings = settings;
        }

        public void RunJob(int index)
        {
            var job = _jobs[index];

            if (BusinessSoftwareDetector.IsRunning(_settings.BusinessSoftware))
                throw new InvalidOperationException($"Job '{job.Name}' stopped: business software detected.");

            IBackupStrategy strategy = job.Type == BackupType.Full
                ? new FullBackupStrategy()
                : new DifferentialBackupStrategy();

            strategy.Execute(job, _daily, _state, _settings);
        }

        public Task RunJobAsync(int index)
        {
            var job = _jobs[index];

            if (BusinessSoftwareDetector.IsRunning(_settings.BusinessSoftware))
                throw new InvalidOperationException($"Job '{job.Name}' stopped: business software detected.");

            job.State = JobState.Running;
            job.Progress = 0;
            job.TotalSizeCopied = 0;
            job.PauseEvent.Set();

            return Task.Run(() =>
            {
                try
                {
                    IBackupStrategy strategy = job.Type == BackupType.Full
                        ? new FullBackupStrategy()
                        : new DifferentialBackupStrategy();

                    strategy.Execute(job, _daily, _state, _settings);

                    if (job.State != JobState.Stopped)
                    {
                        job.State = JobState.Completed;
                        job.Progress = 100.0;
                    }
                }
                catch (Exception ex)
                {
                    job.State = JobState.Error;
                    Console.Error.WriteLine($"Error in job {job.Name}: {ex.Message}");
                }
            });
        }

        public void RunAllJobs()
        {
            if (_jobs.Count == 0) { Console.WriteLine(_lang.Get("run_all_none")); return; }
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < _jobs.Count; i++)
                tasks.Add(RunJobAsync(i));
        }

        public void RunFromArgument(string arg)
        {
            foreach (int idx in ParseArgument(arg))
            {
                if (idx < 1 || idx > _jobs.Count)
                    Console.Error.WriteLine($"Index {idx} out of range.");
                else
                    RunJobAsync(idx - 1);
            }
        }

        public void PauseJob(int index)
        {
            var job = _jobs[index];
            if (job.State == JobState.Running)
            {
                job.State = JobState.Paused;
                job.PauseEvent.Reset();
            }
        }

        public void ResumeJob(int index)
        {
            var job = _jobs[index];
            if (job.State == JobState.Paused)
            {
                job.State = JobState.Running;
                job.PauseEvent.Set();
            }
        }

        public void StopJob(int index)
        {
            var job = _jobs[index];
            job.State = JobState.Stopped;
            job.PauseEvent.Set();
        }

        private static IEnumerable<int> ParseArgument(string arg)
        {
            if (arg.Contains('-'))
            {
                var parts = arg.Split('-');
                if (parts.Length == 2
                    && int.TryParse(parts[0], out int from)
                    && int.TryParse(parts[1], out int to))
                    for (int i = from; i <= to; i++) yield return i;
            }
            else
            {
                foreach (string part in arg.Split(';'))
                    if (int.TryParse(part.Trim(), out int n)) yield return n;
            }
        }
    }
}