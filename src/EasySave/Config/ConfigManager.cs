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

        public List<BackupJob> LoadJobs()
        {
            if (!File.Exists(_path)) return new List<BackupJob>();

            try
            {
                return JsonSerializer.Deserialize<List<BackupJob>>(File.ReadAllText(_path), _json) ?? new();
            }
            catch (JsonException)
            {
                return new List<BackupJob>();
            }
        }

        public void SaveJobs(List<BackupJob> jobs) =>
            File.WriteAllText(_path, JsonSerializer.Serialize(jobs, _json));
    }
}
