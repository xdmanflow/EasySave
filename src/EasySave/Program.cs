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
            string root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EasySave");
            string logsDir = Path.Combine(root, "Logs");
            string stateDir = Path.Combine(root, "State");
            string langFile = Path.Combine(root, "lang.txt");

            var config = new ConfigManager();
            var appSettings = config.LoadConfig(); // Load the new wrapper
            var jobs = appSettings.Jobs;
            var format = appSettings.LogFormat;

            var langCode = File.Exists(langFile) ? File.ReadAllText(langFile).Trim() : "en";
            var lang = new LanguageManager(langCode);

            // Inject the configured format into the loggers
            var daily = new DailyLogger(logsDir, format);
            var state = new StateLogger(stateDir, jobs.Select(j => j.Name), format);
            var manager = new BackupManager(jobs, daily, state, lang);

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
                Console.WriteLine(lang.Get("menu_log_format"));
                Console.WriteLine(lang.Get("menu_exit"));
                Console.WriteLine();
                Console.Write(lang.Get("menu_choice"));

                switch (Console.ReadLine())
                {
                    case "1":
                        if (jobs.Count == 0)
                            Console.WriteLine(lang.Get("no_jobs"));
                        else
                            for (int i = 0; i < jobs.Count; i++)
                                Console.WriteLine(lang.Get("job_number", i + 1, jobs[i].Name, jobs[i].SourceDirectory, jobs[i].TargetDirectory, jobs[i].Type));
                        Pause(lang);
                        break;

                    case "2":
                        if (jobs.Count >= MaxJobs) { Console.WriteLine(lang.Get("add_max")); Pause(lang); break; }

                        Console.Write(lang.Get("add_name"));
                        string name = Console.ReadLine()?.Trim() ?? "";

                        if (jobs.Any(j => j.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                        { Console.WriteLine(lang.Get("add_duplicate")); Pause(lang); break; }

                        Console.Write(lang.Get("add_source"));
                        string src = Console.ReadLine()?.Trim() ?? "";

                        Console.Write(lang.Get("add_target"));
                        string tgt = Console.ReadLine()?.Trim() ?? "";

                        Console.Write(lang.Get("add_type"));
                        string typeIn = Console.ReadLine()?.Trim() ?? "1";

                        jobs.Add(new BackupJob
                        {
                            Name = name,
                            SourceDirectory = src,
                            TargetDirectory = tgt,
                            Type = typeIn == "2" ? BackupType.Differential : BackupType.Full
                        });

                        config.SaveConfig(appSettings); // Save updated settings
                        state = new StateLogger(stateDir, jobs.Select(j => j.Name), format);
                        manager = new BackupManager(jobs, daily, state, lang);

                        Console.WriteLine(lang.Get("add_success", name));
                        Pause(lang);
                        break;

                    case "3":
                        if (jobs.Count == 0) { Console.WriteLine(lang.Get("no_jobs")); Pause(lang); break; }

                        Console.Write(lang.Get("delete_index", jobs.Count));
                        if (int.TryParse(Console.ReadLine(), out int di) && di >= 1 && di <= jobs.Count)
                        {
                            string deleted = jobs[di - 1].Name;
                            jobs.RemoveAt(di - 1);
                            config.SaveConfig(appSettings); // Save updated settings
                            state = new StateLogger(stateDir, jobs.Select(j => j.Name), format);
                            manager = new BackupManager(jobs, daily, state, lang);
                            Console.WriteLine(lang.Get("delete_success", deleted));
                        }
                        else Console.WriteLine(lang.Get("invalid_choice"));
                        Pause(lang);
                        break;

                    case "4":
                        if (jobs.Count == 0) { Console.WriteLine(lang.Get("no_jobs")); Pause(lang); break; }

                        Console.Write(lang.Get("run_index", jobs.Count));
                        if (int.TryParse(Console.ReadLine(), out int ri) && ri >= 1 && ri <= jobs.Count)
                            manager.RunJob(ri - 1);
                        else
                            Console.WriteLine(lang.Get("invalid_choice"));
                        Pause(lang);
                        break;

                    case "5":
                        manager.RunAllJobs();
                        Pause(lang);
                        break;

                    case "6":
                        Console.Write(lang.Get("language_prompt"));
                        string? input = Console.ReadLine()?.Trim().ToLower();
                        if (input is "en" or "fr" or "es")
                        {
                            langCode = input;
                            File.WriteAllText(langFile, langCode);
                            lang = new LanguageManager(langCode);
                            manager = new BackupManager(jobs, daily, state, lang);
                        }
                        else Console.WriteLine(lang.Get("invalid_choice"));
                        Pause(lang);
                        break;

                    case "7":
                        // New Case for Log Format Switching
                        Console.WriteLine($"Current format: {appSettings.LogFormat}");
                        Console.Write(lang.Get("log_format_prompt"));
                        string? fmtInput = Console.ReadLine()?.Trim();

                        if (fmtInput == "1" || fmtInput == "2")
                        {
                            format = fmtInput == "2" ? LogFormat.XML : LogFormat.JSON;
                            appSettings.LogFormat = format;
                            config.SaveConfig(appSettings);

                            // Re-instantiate the loggers and manager to apply the new format
                            daily = new DailyLogger(logsDir, format);
                            state = new StateLogger(stateDir, jobs.Select(j => j.Name), format);
                            manager = new BackupManager(jobs, daily, state, lang);

                            Console.WriteLine(lang.Get("log_format_success"));
                        }
                        else
                        {
                            Console.WriteLine(lang.Get("invalid_choice"));
                        }
                        Pause(lang);
                        break;

                    case "0":
                        running = false;
                        break;

                    default:
                        Console.WriteLine(lang.Get("invalid_choice"));
                        Pause(lang);
                        break;
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
