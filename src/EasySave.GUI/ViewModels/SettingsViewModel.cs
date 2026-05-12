using System.Collections.ObjectModel;
using System.Windows.Input;
using EasySave.Models;
using EasyLog;

namespace EasySave.GUI.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private LogFormat _logFormat;
        private string _cryptoSoftPath = string.Empty;
        private string _language = "en";
        private string _newBusiness = string.Empty;
        private string _newExtension = string.Empty;
        private string _newPriorityExtension = string.Empty;
        private string? _selectedBusiness;
        private string? _selectedExtension;

        // --- NEW VARIABLES FOR STEP 4 ---
        private long _maxFileSizeKB = 0;
        private string _newPriorityExtension = string.Empty;
        private string? _selectedPriorityExtension;
        private DockerLogMode _dockerLogMode;
        private string _dockerLogUrl = string.Empty;
        private long _maxFileSizeKB = 0;

        public bool IsJson
        {
            get => _logFormat == LogFormat.Json;
            set { if (value) { _logFormat = LogFormat.Json; OnPropertyChanged(); OnPropertyChanged(nameof(IsXml)); } }
        }

        public bool IsXml
        {
            get => _logFormat == LogFormat.Xml;
            set { if (value) { _logFormat = LogFormat.Xml; OnPropertyChanged(); OnPropertyChanged(nameof(IsJson)); } }
        }

        public bool IsLocal
        {
            get => _dockerLogMode == DockerLogMode.Local;
            set { if (value) { _dockerLogMode = DockerLogMode.Local; OnPropertyChanged(); OnPropertyChanged(nameof(IsRemote)); OnPropertyChanged(nameof(IsBoth)); } }
        }

        public bool IsRemote
        {
            get => _dockerLogMode == DockerLogMode.Remote;
            set { if (value) { _dockerLogMode = DockerLogMode.Remote; OnPropertyChanged(); OnPropertyChanged(nameof(IsLocal)); OnPropertyChanged(nameof(IsBoth)); } }
        }

        public bool IsBoth
        {
            get => _dockerLogMode == DockerLogMode.Both;
            set { if (value) { _dockerLogMode = DockerLogMode.Both; OnPropertyChanged(); OnPropertyChanged(nameof(IsLocal)); OnPropertyChanged(nameof(IsRemote)); } }
        }

        public string DockerLogUrl
        {
            get => _dockerLogUrl;
            set => Set(ref _dockerLogUrl, value);
        }

        public string CryptoSoftPath
        {
            get => _cryptoSoftPath;
            set => Set(ref _cryptoSoftPath, value);
        }

        public string Language
        {
            get => _language;
            set => Set(ref _language, value);
        }

        public string NewBusiness
        {
            get => _newBusiness;
            set => Set(ref _newBusiness, value);
        }

        public string NewExtension
        {
            get => _newExtension;
            set => Set(ref _newExtension, value);
        }

        public string NewPriorityExtension
        {
            get => _newPriorityExtension;
            set => Set(ref _newPriorityExtension, value);
        }

        public string? SelectedBusiness
        {
            get => _selectedBusiness;
            set => Set(ref _selectedBusiness, value);
        }

        public string? SelectedExtension
        {
            get => _selectedExtension;
            set => Set(ref _selectedExtension, value);
        }

        // --- NEW PROPERTIES ---
        public long MaxFileSizeKB
        {
            get => _maxFileSizeKB;
            set => Set(ref _maxFileSizeKB, value);
        }

        public string NewPriorityExtension
        {
            get => _newPriorityExtension;
            set => Set(ref _newPriorityExtension, value);
        }

        public string? SelectedPriorityExtension
        {
            get => _selectedPriorityExtension;
            set => Set(ref _selectedPriorityExtension, value);
        }

        public long MaxFileSizeKB
        {
            get => _maxFileSizeKB;
            set => Set(ref _maxFileSizeKB, value);
        }

        public ObservableCollection<string> BusinessSoftwareList { get; } = new();
        public ObservableCollection<string> EncryptedExtensions { get; } = new();

        // --- NEW COLLECTION ---
        public ObservableCollection<string> PriorityExtensions { get; } = new();

        public ICommand AddBusinessCommand { get; }
        public ICommand RemoveBusinessCommand { get; }
        public ICommand AddExtensionCommand { get; }
        public ICommand RemoveExtensionCommand { get; }

        // --- NEW COMMANDS ---
        public ICommand AddPriorityExtensionCommand { get; }
        public ICommand RemovePriorityExtensionCommand { get; }

        public SettingsViewModel(AppSettings s)
        {
            _logFormat = s.LogFormat;
            _cryptoSoftPath = s.CryptoSoftPath;
            _language = s.Language;
            _dockerLogMode = s.DockerLogMode;
            _dockerLogUrl = s.DockerLogUrl;
            _maxFileSizeKB = s.MaxFileSizeKB;

            foreach (var b in s.BusinessSoftware) BusinessSoftwareList.Add(b);
            foreach (var e in s.EncryptedExtensions) EncryptedExtensions.Add(e);
            foreach (var p in s.PriorityExtensions) PriorityExtensions.Add(p);

            AddBusinessCommand = new RelayCommand(_ =>
            {
                if (!string.IsNullOrWhiteSpace(NewBusiness))
                { BusinessSoftwareList.Add(NewBusiness.Trim()); NewBusiness = string.Empty; }
            });

            RemoveBusinessCommand = new RelayCommand(
                _ => { if (SelectedBusiness != null) BusinessSoftwareList.Remove(SelectedBusiness); },
                _ => SelectedBusiness != null);

            AddExtensionCommand = new RelayCommand(_ =>
            {
                if (!string.IsNullOrWhiteSpace(NewExtension))
                {
                    string ext = NewExtension.Trim().ToLowerInvariant();
                    if (!ext.StartsWith(".")) ext = "." + ext;
                    EncryptedExtensions.Add(ext);
                    NewExtension = string.Empty;
                }
            });

            RemoveExtensionCommand = new RelayCommand(
                _ => { if (SelectedExtension != null) EncryptedExtensions.Remove(SelectedExtension); },
                _ => SelectedExtension != null);

            // --- COMMAND BEHAVIORS ---
            AddPriorityExtensionCommand = new RelayCommand(_ =>
            {
                if (!string.IsNullOrWhiteSpace(NewPriorityExtension))
                {
                    string ext = NewPriorityExtension.Trim().ToLowerInvariant();
                    if (!ext.StartsWith(".")) ext = "." + ext;
                    PriorityExtensions.Add(ext);
                    NewPriorityExtension = string.Empty;
                }
            });

            RemovePriorityExtensionCommand = new RelayCommand(
                _ => { if (SelectedPriorityExtension != null) PriorityExtensions.Remove(SelectedPriorityExtension); },
                _ => SelectedPriorityExtension != null);
        }

        public AppSettings ToModel() => new AppSettings
        {
            LogFormat = _logFormat,
            CryptoSoftPath = CryptoSoftPath,
            Language = Language,
            MaxFileSizeKB = MaxFileSizeKB, // Save value
            BusinessSoftware = new System.Collections.Generic.List<string>(BusinessSoftwareList),
            EncryptedExtensions = new System.Collections.Generic.List<string>(EncryptedExtensions),
            DockerLogMode = _dockerLogMode,
            DockerLogUrl = DockerLogUrl,
            PriorityExtensions = new System.Collections.Generic.List<string>(PriorityExtensions),
            MaxFileSizeKB = MaxFileSizeKB
        };
    }
}