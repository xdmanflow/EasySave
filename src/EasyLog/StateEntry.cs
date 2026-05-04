using System.Text.Json.Serialization;

namespace EasyLog
{
    public class StateEntry
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("SourceFilePath")]
        public string SourceFilePath { get; set; } = string.Empty;

        [JsonPropertyName("TargetFilePath")]
        public string TargetFilePath { get; set; } = string.Empty;

        [JsonPropertyName("State")]
        public string State { get; set; } = "END";

        [JsonPropertyName("LastActionTimestamp")]
        public string LastActionTimestamp { get; set; } = string.Empty;

        [JsonPropertyName("TotalFilesToCopy")]
        public int TotalFilesToCopy { get; set; }

        [JsonPropertyName("TotalFilesSize")]
        public long TotalFilesSize { get; set; }

        [JsonPropertyName("NbFilesLeftToDo")]
        public int NbFilesLeftToDo { get; set; }

        [JsonPropertyName("Progression")]
        public int Progression { get; set; }
    }
}
