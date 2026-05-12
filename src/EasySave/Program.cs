using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EasySave.Config;
using EasySave.Languages;
using EasySave.Models;
using EasyLog;

namespace EasySave
{
    internal class Program
    {
        private const int MaxJobs = 5;

        static void Main(string[] args)
        {
            string root = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EasySave");
            string logsDir = Path.Combine(root, "Logs");
            string stateDir = Path.Combine(root, "State");

            var config = new ConfigManager();
            var jobs = config.LoadJobs();
            var settings = config.LoadSettings();
            var lang = new LanguageManager(settings.Language);
            var daily = new DailyLogger(logsDir, settings.LogFormat);
            var state = new StateLogger(stateDir, jobs.Select(j => j.Name));
            var manager = new BackupManager(jobs, daily, state, lang, settings);

            if (args.Length == 1)
            {
                manager.RunFromArgument(args[0]);
                return;
            }

            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine(lang.Get("menu_title"));
                Console.WriteLine();
                Console.WriteLine(lang.Get("menu_list"));
                Console.WriteLine(lang.Get("menu_add"));
                Console.WriteLine(lang.Get("menu_delete"));
                Console.WriteLine(lang.Get("menu_run_one"));
                Console.WriteLine(lang.Get("menu_run_all"));
                Console.WriteLine(lang.Get("menu_language"));
                Console.WriteLine($"7. Log format  (current: {settings.LogFormat})");
                Console.WriteLine(lang.Get("menu_exit"));
                Console.WriteLine();
                Console.Write(lang.Get("menu_choice"));

                switch (Console.ReadLine())
                {
                    case "1":
                        if (jobs.Count == 0) Console.WriteLine(lang.Get("no_jobs"));
                        else for (int i = 0; i < jobs.Count; i++)
                            Console.WriteLine(lang.Get("job_number",
                                i + 1, jobs[i].Name,
                                jobs[i].SourceDirectory,
                                jobs[i].TargetDirectory,
                                jobs[i].Type));
                        Pause(lang); break;

                    case "2":
                        if (jobs.Count >= MaxJobs)
                        { Console.WriteLine(lang.Get("add_max")); Pause(lang); break; }

                        Console.Write(lang.Get("add_name"));
                        string name = Console.ReadLine()?.Trim() ?? "";
                        if (jobs.Any(j => j.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                        { Console.WriteLine(lang.Get("add_duplicate")); Pause(lang); break; }

                        Console.Write(lang.Get("add_source")); string src = Console.ReadLine()?.Trim() ?? "";
                        Console.Write(lang.Get("add_target")); string tgt = Console.ReadLine()?.Trim() ?? "";
                        Console.Write(lang.Get("add_type")); string ti = Console.ReadLine()?.Trim() ?? "1";

                        jobs.Add(new BackupJob
                        {
                            Name = name,
                            SourceDirectory = src,
                            TargetDirectory = tgt,
                            Type = ti == "2" ? BackupType.Differential : BackupType.Full
                        });
                        config.SaveJobs(jobs);
                        state = new StateLogger(stateDir, jobs.Select(j => j.Name));
                        manager = new BackupManager(jobs, daily, state, lang, settings);
                        Console.WriteLine(lang.Get("add_success", name));
                        Pause(lang); break;

                    case "3":
                        if (jobs.Count == 0) { Console.WriteLine(lang.Get("no_jobs")); Pause(lang); break; }
                        Console.Write(lang.Get("delete_index", jobs.Count));
                        if (int.TryParse(Console.ReadLine(), out int di) && di >= 1 && di <= jobs.Count)
                        {
                            string del = jobs[di - 1].Name;
                            jobs.RemoveAt(di - 1);
                            config.SaveJobs(jobs);
                            state = new StateLogger(stateDir, jobs.Select(j => j.Name));
                            manager = new BackupManager(jobs, daily, state, lang, settings);
                            Console.WriteLine(lang.Get("delete_success", del));
                        }
                        else Console.WriteLine(lang.Get("invalid_choice"));
                        Pause(lang); break;

                    case "4":
                        if (jobs.Count == 0) { Console.WriteLine(lang.Get("no_jobs")); Pause(lang); break; }
                        Console.Write(lang.Get("run_index", jobs.Count));
                        if (int.TryParse(Console.ReadLine(), out int ri) && ri >= 1 && ri <= jobs.Count)
                            manager.RunJobAsync(ri - 1);
                        else Console.WriteLine(lang.Get("invalid_choice"));
                        Pause(lang); break;

                    case "5":
                        manager.RunAllJobs();
                        Pause(lang); break;

                    case "6":
                        Console.Write(lang.Get("language_prompt"));
                        string? inp = Console.ReadLine()?.Trim().ToLower();
                        if (inp is "en" or "fr" or "es")
                        {
                            settings.Language = inp;
                            config.SaveSettings(settings);
                            lang = new LanguageManager(inp);
                            manager = new BackupManager(jobs, daily, state, lang, settings);
                        }
                        else Console.WriteLine(lang.Get("invalid_choice"));
                        Pause(lang); break;

                    case "7":
                        Console.WriteLine("Log format: 1 = JSON   2 = XML");
                        string? fmt = Console.ReadLine()?.Trim();
                        if (fmt == "1") { settings.LogFormat = LogFormat.Json; daily.SetFormat(LogFormat.Json); }
                        else if (fmt == "2") { settings.LogFormat = LogFormat.Xml; daily.SetFormat(LogFormat.Xml); }
                        else Console.WriteLine(lang.Get("invalid_choice"));
                        config.SaveSettings(settings);
                        Pause(lang); break;

                    case "0":
                        running = false; break;

                    default:
                        Console.WriteLine(lang.Get("invalid_choice"));
                        Pause(lang); break;
                }
            }
        }

        static void Pause(LanguageManager lang)
        {
            Console.WriteLine();
            Console.WriteLine(lang.Get("press_enter"));
            Console.ReadLine();
        }
    }
}