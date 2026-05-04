using System.Collections.Generic;
using EasyLog;
using EasySave.Models;

namespace EasySave.Config
{
    public class AppSettings
    {
        public List<BackupJob> Jobs { get; set; } = new();
        public LogFormat LogFormat { get; set; } = LogFormat.JSON;
    }
}