using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using EasySave.Models;

namespace EasySave.Config
{
    public class ConfigManager
    {
        private readonly string _path;

        private static readonly JsonSerializerOptions _json = new() { WriteIndented = true };

        public ConfigManager()
        {
            string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EasySave");
            Directory.CreateDirectory(dir);
            _path = Path.Combine(dir, "config.json");
        }

        public AppSettings LoadConfig()
        {
            if (!File.Exists(_path)) return new AppSettings();

            string jsonStr = File.ReadAllText(_path);
            try
            {
                // Try to load as v1.1 AppSettings
                return JsonSerializer.Deserialize<AppSettings>(jsonStr, _json) ?? new AppSettings();
            }
            catch (JsonException)
            {
                try
                {
                    // Fallback: If it fails, it might be a v1.0 config (just a JSON array of jobs).
                    // Migrate it to the new AppSettings format.
                    var legacyJobs = JsonSerializer.Deserialize<List<BackupJob>>(jsonStr, _json) ?? new List<BackupJob>();
                    return new AppSettings { Jobs = legacyJobs };
                }
                catch
                {
                    return new AppSettings();
                }
            }
        }

        public void SaveConfig(AppSettings settings) =>
            File.WriteAllText(_path, JsonSerializer.Serialize(settings, _json));
    }
}
