using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace EasyLog
{
    public class LogEntry
    {
        [JsonPropertyName("Name")]
        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("FileSource")]
        [XmlElement("FileSource")]
        public string FileSource { get; set; } = string.Empty;

        [JsonPropertyName("FileTarget")]
        [XmlElement("FileTarget")]
        public string FileTarget { get; set; } = string.Empty;

        [JsonPropertyName("FileSize")]
        [XmlElement("FileSize")]
        public long FileSize { get; set; }

        [JsonPropertyName("FileTransferTime")]
        [XmlElement("FileTransferTime")]
        public double FileTransferTime { get; set; }

        [JsonPropertyName("Time")]
        [XmlElement("Time")]
        public string Time { get; set; } = string.Empty;
    }
}
