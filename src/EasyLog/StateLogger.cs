using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Serialization;

namespace EasyLog
{
    public class StateLogger
    {
        private readonly string _filePath;
        private readonly List<StateEntry> _slots;
        private readonly LogFormat _format;

        private static readonly JsonSerializerOptions _json = new() { WriteIndented = true };
        private static readonly XmlSerializer _xmlSerializer = new(typeof(List<StateEntry>));

        public StateLogger(string folder, IEnumerable<string> jobNames, LogFormat format = LogFormat.JSON)
        {
            _format = format;
            Directory.CreateDirectory(folder);

            string ext = _format == LogFormat.XML ? ".xml" : ".json";
            _filePath = Path.Combine(folder, $"state{ext}");

            _slots = jobNames.Select(n => new StateEntry { Name = n }).ToList();
            Save();
        }

        public void Update(StateEntry updated)
        {
            updated.LastActionTimestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            int i = _slots.FindIndex(s => s.Name == updated.Name);

            if (i >= 0) _slots[i] = updated;
            else _slots.Add(updated);

            Save();
        }

        public void SetEnd(string name)
        {
            int i = _slots.FindIndex(s => s.Name == name);
            if (i >= 0)
                _slots[i] = new StateEntry
                {
                    Name = name,
                    State = "END",
                    LastActionTimestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                };
            Save();
        }

        private void Save()
        {
            if (_format == LogFormat.JSON)
            {
                File.WriteAllText(_filePath, JsonSerializer.Serialize(_slots, _json));
            }
            else
            {
                using var writer = new StreamWriter(_filePath);
                _xmlSerializer.Serialize(writer, _slots);
            }
        }
    }
}
