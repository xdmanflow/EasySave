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
        private LogFormat _format;
        private static readonly JsonSerializerOptions _json = new() { WriteIndented = true };

        public DailyLogger(string folder, LogFormat format = LogFormat.Json)
        {
            _folder = folder;
            _format = format;
            Directory.CreateDirectory(folder);
        }

        public void SetFormat(LogFormat format) => _format = format;

        public void Log(LogEntry entry)
        {
            entry.Time = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            if (_format == LogFormat.Json) LogJson(entry);
            else LogXml(entry);
        }

        private void LogJson(LogEntry entry)
        {
            string path = Path.Combine(_folder, $"{DateTime.Now:yyyy-MM-dd}.json");
            var list = new List<LogEntry>();
            if (File.Exists(path))
            {
                try { list = JsonSerializer.Deserialize<List<LogEntry>>(File.ReadAllText(path), _json) ?? new(); }
                catch (JsonException) { list = new(); }
            }
            list.Add(entry);
            File.WriteAllText(path, JsonSerializer.Serialize(list, _json));
        }

        private void LogXml(LogEntry entry)
        {
            string path = Path.Combine(_folder, $"{DateTime.Now:yyyy-MM-dd}.xml");
            var list = new List<LogEntry>();
            var ser = new XmlSerializer(typeof(List<LogEntry>), new XmlRootAttribute("LogEntries"));
            if (File.Exists(path))
            {
                try { using var r = new StreamReader(path); list = (List<LogEntry>?)ser.Deserialize(r) ?? new(); }
                catch { list = new(); }
            }
            list.Add(entry);
            using var w = new StreamWriter(path);
            ser.Serialize(w, list);
        }
    }
}