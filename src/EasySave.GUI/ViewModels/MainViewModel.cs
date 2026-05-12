using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
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
        private List<BackupJob> _backendJobs = new();

        private JobViewModel? _selectedJob;
        private string _statusMessage = "Ready.";
        private readonly DispatcherTimer _progressTimer;

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
        public ICommand PauseJobCommand { get; }
        public ICommand ResumeJobCommand { get; }
        public ICommand StopJobCommand { get; }

        public event Action<JobViewModel?>? RequestEditJob;
        public event Action? RequestOpenSettings;

        public MainViewModel()
        {
            _config = new ConfigManager();
            _settings = _config.LoadSettings();

            string root = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EasySave");
            _lang = new LanguageManager(_settings.Language);
            _daily = new DailyLogger(
                Path.Combine(root, "Logs"),
                _settings.LogFormat,
                _settings.DockerLogMode,
                _settings.DockerLogUrl);
            _state = new StateLogger(Path.Combine(root, "State"), Enumerable.Empty<string>());

            AddJobCommand = new RelayCommand(_ => RequestEditJob?.Invoke(null));
            EditJobCommand = new RelayCommand(_ => RequestEditJob?.Invoke(SelectedJob), _ => SelectedJob != null);
            DeleteJobCommand = new RelayCommand(_ => DeleteJob(), _ => SelectedJob != null);
            OpenSettingsCommand = new RelayCommand(_ => RequestOpenSettings?.Invoke());

            RunJobCommand = new RelayCommand(_ => RunSelected(), _ => SelectedJob != null && SelectedJob.State != JobState.Running);
            RunAllJobsCommand = new RelayCommand(_ => RunAll(), _ => Jobs.Count > 0);

            PauseJobCommand = new RelayCommand(_ => PauseSelected(), _ => SelectedJob != null && SelectedJob.State == JobState.Running);
            ResumeJobCommand = new RelayCommand(_ => ResumeSelected(), _ => SelectedJob != null && SelectedJob.State == JobState.Paused);
            StopJobCommand = new RelayCommand(_ => StopSelected(), _ => SelectedJob != null && (SelectedJob.State == JobState.Running || SelectedJob.State == JobState.Paused));

            _progressTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
            _progressTimer.Tick += SyncProgress;
            _progressTimer.Start();

            LoadJobs();
        }

        private void SyncProgress(object? sender, EventArgs e)
        {
            for (int i = 0; i < Jobs.Count; i++)
            {
                if (i < _backendJobs.Count)
                {
                    Jobs[i].Progress = _backendJobs[i].Progress;
                    Jobs[i].State = _backendJobs[i].State;
                }
            }
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
            _backendJobs = Jobs.Select(j => j.ToModel()).ToList();
            string stateDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "EasySave", "State");
            _state = new StateLogger(stateDir, _backendJobs.Select(j => j.Name));
            _manager = new BackupManager(_backendJobs, _daily, _state, _lang, _settings);
        }

        public void SaveJob(JobViewModel vm, bool isNew, string? originalName)
        {
            if (isNew) Jobs.Add(vm);
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
            int idx = Jobs.IndexOf(SelectedJob);
            try
            {
                _manager.RunJobAsync(idx);
                StatusMessage = $"Job '{SelectedJob.Name}' started.";
            }
            catch (Exception ex) { StatusMessage = $"Error: {ex.Message}"; }
        }

        private void RunAll()
        {
            try
            {
                _manager.RunAllJobs();
                StatusMessage = "All jobs started.";
            }
            catch (Exception ex) { StatusMessage = $"Error: {ex.Message}"; }
        }

        private void PauseSelected()
        {
            if (SelectedJob != null) _manager.PauseJob(Jobs.IndexOf(SelectedJob));
        }

        private void ResumeSelected()
        {
            if (SelectedJob != null) _manager.ResumeJob(Jobs.IndexOf(SelectedJob));
        }

        private void StopSelected()
        {
            if (SelectedJob != null) _manager.StopJob(Jobs.IndexOf(SelectedJob));
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
            _daily.SetDockerConfig(s.DockerLogMode, s.DockerLogUrl);
            _lang = new LanguageManager(s.Language);
            RefreshManager();
            StatusMessage = "Settings saved.";
        }

        public AppSettings GetSettings() => _settings;
    }
}