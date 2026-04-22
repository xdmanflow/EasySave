using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EasyLog
{
    public class DailyLogger
    {
        private readonly string _folder;

        private static readonly JsonSerializerOptions _json = new() { WriteIndented = true };

        public DailyLogger(string folder)
        {
            _folder = folder;
            Directory.CreateDirectory(folder);
        }

        public void Log(LogEntry entry)
        {
            entry.Time = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            string path = Path.Combine(_folder, $"{DateTime.Now:yyyy-MM-dd}.json");

            var existing = new List<LogEntry>();

            if (File.Exists(path))
            {
                try { existing = JsonSerializer.Deserialize<List<LogEntry>>(File.ReadAllText(path), _json) ?? new(); }
                catch (JsonException) { existing = new(); }
            }

            existing.Add(entry);
            File.WriteAllText(path, JsonSerializer.Serialize(existing, _json));
        }
    }
}
