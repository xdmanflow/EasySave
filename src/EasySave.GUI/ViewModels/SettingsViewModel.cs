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
        private string? _selectedBusiness;
        private string? _selectedExtension;

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

        public ObservableCollection<string> BusinessSoftwareList { get; } = new();
        public ObservableCollection<string> EncryptedExtensions { get; } = new();

        public ICommand AddBusinessCommand { get; }
        public ICommand RemoveBusinessCommand { get; }
        public ICommand AddExtensionCommand { get; }
        public ICommand RemoveExtensionCommand { get; }

        public SettingsViewModel(AppSettings s)
        {
            _logFormat = s.LogFormat;
            _cryptoSoftPath = s.CryptoSoftPath;
            _language = s.Language;

            foreach (var b in s.BusinessSoftware) BusinessSoftwareList.Add(b);
            foreach (var e in s.EncryptedExtensions) EncryptedExtensions.Add(e);

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
        }

        public AppSettings ToModel() => new AppSettings
        {
            LogFormat = _logFormat,
            CryptoSoftPath = CryptoSoftPath,
            Language = Language,
            BusinessSoftware = new System.Collections.Generic.List<string>(BusinessSoftwareList),
            EncryptedExtensions = new System.Collections.Generic.List<string>(EncryptedExtensions)
        };
    }
}