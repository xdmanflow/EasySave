using System.Collections.Generic;
using System.Text.Json.Serialization;
using EasyLog;

namespace EasySave.Models
{
    public class AppSettings
    {
        [JsonPropertyName("LogFormat")]
        public LogFormat LogFormat { get; set; } = LogFormat.Json;

        [JsonPropertyName("Language")]
        public string Language { get; set; } = "en";

        [JsonPropertyName("CryptoSoftPath")]
        public string CryptoSoftPath { get; set; } = string.Empty;

        [JsonPropertyName("BusinessSoftware")]
        public List<string> BusinessSoftware { get; set; } = new();

        [JsonPropertyName("EncryptedExtensions")]
        public List<string> EncryptedExtensions { get; set; } = new();

        [JsonPropertyName("PriorityExtensions")]
        public List<string> PriorityExtensions { get; set; } = new();

        [JsonPropertyName("MaxFileSizeKB")]
        public long MaxFileSizeKB { get; set; } = 0;

        [JsonPropertyName("DockerLogMode")]
        public DockerLogMode DockerLogMode { get; set; } = DockerLogMode.Local;

        [JsonPropertyName("DockerLogUrl")]
        public string DockerLogUrl { get; set; } = "http://localhost:5050/log";
    }
}