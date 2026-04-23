using System.Text.Json.Serialization;

namespace EasySave.Models
{
    public class BackupJob
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("SourceDirectory")]
        public string SourceDirectory { get; set; } = string.Empty;

        [JsonPropertyName("TargetDirectory")]
        public string TargetDirectory { get; set; } = string.Empty;

        [JsonPropertyName("Type")]
        public BackupType Type { get; set; } = BackupType.Full;
    }
}
