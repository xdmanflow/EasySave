using System;
using EasySave.Models;

namespace EasySave.GUI.ViewModels
{
    public class JobViewModel : ViewModelBase
    {
        private string _name = string.Empty;
        private string _sourceDirectory = string.Empty;
        private string _targetDirectory = string.Empty;
        private BackupType _type = BackupType.Full;

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public string SourceDirectory
        {
            get => _sourceDirectory;
            set => Set(ref _sourceDirectory, value);
        }

        public string TargetDirectory
        {
            get => _targetDirectory;
            set => Set(ref _targetDirectory, value);
        }

        public BackupType Type
        {
            get => _type;
            set => Set(ref _type, value);
        }

        public Array BackupTypes { get; } = Enum.GetValues(typeof(BackupType));

        public BackupJob ToModel() => new BackupJob
        {
            Name = Name,
            SourceDirectory = SourceDirectory,
            TargetDirectory = TargetDirectory,
            Type = Type
        };

        public static JobViewModel FromModel(BackupJob job) => new JobViewModel
        {
            Name = job.Name,
            SourceDirectory = job.SourceDirectory,
            TargetDirectory = job.TargetDirectory,
            Type = job.Type
        };
    }
}