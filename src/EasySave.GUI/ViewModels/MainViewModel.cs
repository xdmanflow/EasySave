using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using EasySave.Config;
using EasySave.Languages;
using EasySave.Models;
using EasyLog;

namespace EasySave.GUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ConfigManager _config;
        private AppSettings _settings;
        private DailyLogger _daily;
        private StateLogger _state;
        private LanguageManager _lang;
        private BackupManager _manager = null!;

        private JobViewModel? _selectedJob;
        private string _statusMessage = "Ready.";

        public ObservableCollection<JobViewModel> Jobs { get; } = new();

        public JobViewModel? SelectedJob
        {
            get => _selectedJob;
            set => Set(ref _selectedJob, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => Set(ref _statusMessage, value);
        }

        public ICommand AddJobCommand { get; }
        public ICommand EditJobCommand { get; }
        public ICommand DeleteJobCommand { get; }
        public ICommand RunJobCommand { get; }
        public ICommand RunAllJobsCommand { get; }
        public ICommand OpenSettingsCommand { get; }

        public event Action<JobViewModel?>? RequestEditJob;
        public event Action? RequestOpenSettings;

        public MainViewModel()
        {
            _config = new ConfigManager();
            _settings = _config.LoadSettings();

            string root = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EasySave");
            _lang = new LanguageManager(_settings.Language);
            _daily = new DailyLogger(Path.Combine(root, "Logs"), _settings.LogFormat);
            _state = new StateLogger(Path.Combine(root, "State"), Enumerable.Empty<string>());

            AddJobCommand = new RelayCommand(_ => RequestEditJob?.Invoke(null));
            EditJobCommand = new RelayCommand(_ => RequestEditJob?.Invoke(SelectedJob),
                                                   _ => SelectedJob != null);
            DeleteJobCommand = new RelayCommand(_ => DeleteJob(), _ => SelectedJob != null);
            RunJobCommand = new RelayCommand(_ => RunSelected(), _ => SelectedJob != null);
            RunAllJobsCommand = new RelayCommand(_ => RunAll(), _ => Jobs.Count > 0);
            OpenSettingsCommand = new RelayCommand(_ => RequestOpenSettings?.Invoke());

            LoadJobs();
        }

        private void LoadJobs()
        {
            Jobs.Clear();
            foreach (var job in _config.LoadJobs())
                Jobs.Add(JobViewModel.FromModel(job));
            RefreshManager();
        }

        private void RefreshManager()
        {
            var list = Jobs.Select(j => j.ToModel()).ToList();
            string stateDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "EasySave", "State");
            _state = new StateLogger(stateDir, list.Select(j => j.Name));
            _manager = new BackupManager(list, _daily, _state, _lang, _settings);
        }

        public void SaveJob(JobViewModel vm, bool isNew, string? originalName)
        {
            if (isNew)
            {
                Jobs.Add(vm);
            }
            else
            {
                var existing = Jobs.FirstOrDefault(j => j.Name == originalName);
                if (existing != null)
                {
                    existing.Name = vm.Name;
                    existing.SourceDirectory = vm.SourceDirectory;
                    existing.TargetDirectory = vm.TargetDirectory;
                    existing.Type = vm.Type;
                }
            }
            PersistJobs();
        }

        private void DeleteJob()
        {
            if (SelectedJob == null) return;
            Jobs.Remove(SelectedJob);
            SelectedJob = null;
            PersistJobs();
        }

        private void RunSelected()
        {
            if (SelectedJob == null) return;
            RefreshManager();
            int idx = Jobs.IndexOf(SelectedJob);
            try
            {
                _manager.RunJob(idx);
                StatusMessage = $"Job '{SelectedJob.Name}' completed.";
            }
            catch (Exception ex) { StatusMessage = $"Error: {ex.Message}"; }
        }

        private void RunAll()
        {
            RefreshManager();
            try { _manager.RunAllJobs(); StatusMessage = "All jobs completed."; }
            catch (Exception ex) { StatusMessage = $"Error: {ex.Message}"; }
        }

        private void PersistJobs()
        {
            _config.SaveJobs(Jobs.Select(j => j.ToModel()).ToList());
            RefreshManager();
        }

        public void ApplySettings(AppSettings s)
        {
            _settings = s;
            _config.SaveSettings(s);
            _daily.SetFormat(s.LogFormat);
            _lang = new LanguageManager(s.Language);
            RefreshManager();
            StatusMessage = "Settings saved.";
        }

        public AppSettings GetSettings() => _settings;
    }
}