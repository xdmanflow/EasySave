using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace EasyLog
{
    public class StateEntry
    {
        [JsonPropertyName("Name")]
        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("SourceFilePath")]
        [XmlElement("SourceFilePath")]
        public string SourceFilePath { get; set; } = string.Empty;

        [JsonPropertyName("TargetFilePath")]
        [XmlElement("TargetFilePath")]
        public string TargetFilePath { get; set; } = string.Empty;

        [JsonPropertyName("State")]
        [XmlElement("State")]
        public string State { get; set; } = "END";

        [JsonPropertyName("LastActionTimestamp")]
        [XmlElement("LastActionTimestamp")]
        public string LastActionTimestamp { get; set; } = string.Empty;

        [JsonPropertyName("TotalFilesToCopy")]
        [XmlElement("TotalFilesToCopy")]
        public int TotalFilesToCopy { get; set; }

        [JsonPropertyName("TotalFilesSize")]
        [XmlElement("TotalFilesSize")]
        public long TotalFilesSize { get; set; }

        [JsonPropertyName("NbFilesLeftToDo")]
        [XmlElement("NbFilesLeftToDo")]
        public int NbFilesLeftToDo { get; set; }

        [JsonPropertyName("Progression")]
        [XmlElement("Progression")]
        public int Progression { get; set; }
    }
}