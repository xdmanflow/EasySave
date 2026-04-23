using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace EasyLog
{
    public class StateLogger
    {
        private readonly string _filePath;
        private readonly List<StateEntry> _slots;

        private static readonly JsonSerializerOptions _json = new() { WriteIndented = true };

        public StateLogger(string folder, IEnumerable<string> jobNames)
        {
            Directory.CreateDirectory(folder);
            _filePath = Path.Combine(folder, "state.json");
            _slots = jobNames.Select(n => new StateEntry { Name = n }).ToList();
            Save();
        }

        public void Update(StateEntry updated)
        {
            updated.LastActionTimestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            int i = _slots.FindIndex(s => s.Name == updated.Name);

            if (i >= 0)
                _slots[i] = updated;
            else
                _slots.Add(updated);

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

        private void Save() => File.WriteAllText(_filePath, JsonSerializer.Serialize(_slots, _json));
    }
}
