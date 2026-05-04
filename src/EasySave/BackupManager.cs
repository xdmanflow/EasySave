using System;
using System.Collections.Generic;
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

        public BackupManager(List<BackupJob> jobs, DailyLogger daily, StateLogger state, LanguageManager lang)
        {
            _jobs  = jobs;
            _daily = daily;
            _state = state;
            _lang  = lang;
        }

        public void RunJob(int index)
        {
            var job = _jobs[index];
            Console.WriteLine(_lang.Get("run_start", job.Name));

            IBackupStrategy strategy = job.Type == BackupType.Full
                ? new FullBackupStrategy()
                : (IBackupStrategy)new DifferentialBackupStrategy();

            try
            {
                strategy.Execute(job, _daily, _state);
                Console.WriteLine(_lang.Get("run_done", job.Name));
            }
            catch (Exception ex)
            {
                Console.WriteLine(_lang.Get("run_error", job.Name, ex.Message));
            }
        }

        public void RunAllJobs()
        {
            if (_jobs.Count == 0)
            {
                Console.WriteLine(_lang.Get("run_all_none"));
                return;
            }

            for (int i = 0; i < _jobs.Count; i++)
                RunJob(i);
        }

        public void RunFromArgument(string arg)
        {
            foreach (int idx in ParseArgument(arg))
            {
                if (idx < 1 || idx > _jobs.Count)
                    Console.Error.WriteLine($"Index {idx} out of range.");
                else
                    RunJob(idx - 1);
            }
        }

        private static IEnumerable<int> ParseArgument(string arg)
        {
            if (arg.Contains('-'))
            {
                var parts = arg.Split('-');
                if (parts.Length == 2 && int.TryParse(parts[0], out int from) && int.TryParse(parts[1], out int to))
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
