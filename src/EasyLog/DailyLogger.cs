using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;

namespace EasyLog
{
    public class DailyLogger
    {
        private readonly string _folder;
        private readonly LogFormat _format;
        private static readonly JsonSerializerOptions _json = new() { WriteIndented = true };
        private static readonly XmlSerializer _xmlSerializer = new(typeof(List<LogEntry>));

        public DailyLogger(string folder, LogFormat format = LogFormat.JSON)
        {
            _folder = folder;
            _format = format;
            Directory.CreateDirectory(folder);
        }

        public void Log(LogEntry entry)
        {
            entry.Time = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            string ext = _format == LogFormat.XML ? ".xml" : ".json";
            string path = Path.Combine(_folder, $"{DateTime.Now:yyyy-MM-dd}{ext}");

            var existing = new List<LogEntry>();

            if (File.Exists(path))
            {
                try
                {
                    if (_format == LogFormat.JSON)
                    {
                        existing = JsonSerializer.Deserialize<List<LogEntry>>(File.ReadAllText(path), _json) ?? new();
                    }
                    else
                    {
                        using var reader = new StreamReader(path);
                        existing = (List<LogEntry>)_xmlSerializer.Deserialize(reader)! ?? new();
                    }
                }
                catch { existing = new(); }
            }

            existing.Add(entry);

            if (_format == LogFormat.JSON)
            {
                File.WriteAllText(path, JsonSerializer.Serialize(existing, _json));
            }
            else
            {
                using var writer = new StreamWriter(path);
                _xmlSerializer.Serialize(writer, existing);
            }
        }
    }
}
