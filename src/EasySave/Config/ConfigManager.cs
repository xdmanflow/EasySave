using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using EasySave.Models;

namespace EasySave.Config
{
    
    public class ConfigManager
    {
        private readonly string _jobsPath;
        private readonly string _settingsPath;
        private static readonly JsonSerializerOptions _json = new() { WriteIndented = true };

        public ConfigManager()
        {
            string dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EasySave");
            Directory.CreateDirectory(dir);
            _jobsPath    = Path.Combine(dir, "config.json");
            _settingsPath = Path.Combine(dir, "settings.json");
        }

        public List<BackupJob> LoadJobs()
        {
            if (!File.Exists(_jobsPath)) return new();
            try { return JsonSerializer.Deserialize<List<BackupJob>>(File.ReadAllText(_jobsPath), _json) ?? new(); }
            catch (JsonException) { return new(); }
        }

        public void SaveJobs(List<BackupJob> jobs) =>
            File.WriteAllText(_jobsPath, JsonSerializer.Serialize(jobs, _json));

        public AppSettings LoadSettings()
        {
            if (!File.Exists(_settingsPath)) return new();
            try { return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(_settingsPath), _json) ?? new(); }
            catch (JsonException) { return new(); }
        }

        public void SaveSettings(AppSettings settings) =>
            File.WriteAllText(_settingsPath, JsonSerializer.Serialize(settings, _json));
    }
}