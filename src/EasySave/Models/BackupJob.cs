using System.Text.Json.Serialization;
using System.Threading;

namespace EasySave.Models
{
    public enum JobState { Idle, Running, Paused, Stopped, Completed, Error }

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

        [JsonIgnore]
        public JobState State { get; set; } = JobState.Idle;

        [JsonIgnore]
        public double Progress { get; set; } = 0.0;

        [JsonIgnore]
        public long TotalFilesSize { get; set; }

        [JsonIgnore]
        public long TotalSizeCopied { get; set; }

        [JsonIgnore]
        public ManualResetEventSlim PauseEvent { get; } = new ManualResetEventSlim(true);
    }
}