using System.Windows;
using Microsoft.Win32;
using EasySave.GUI.ViewModels;

namespace EasySave.GUI.Views
{
    public partial class JobEditWindow : Window
    {
        private readonly JobViewModel  _vm;
        private readonly MainViewModel _main;
        private readonly bool          _isNew;
        private readonly string?       _originalName;

        public JobEditWindow(JobViewModel? existing, MainViewModel main)
        {
            InitializeComponent();
            _main         = main;
            _isNew        = existing == null;
            _originalName = existing?.Name;
            _vm = existing == null ? new JobViewModel()
                : new JobViewModel
                  {
                      Name            = existing.Name,
                      SourceDirectory = existing.SourceDirectory,
                      TargetDirectory = existing.TargetDirectory,
                      Type            = existing.Type
                  };
            DataContext = _vm;
        }

        private void BrowseSource_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFolderDialog { Title = "Select source folder" };
            if (dlg.ShowDialog() == true) _vm.SourceDirectory = dlg.FolderName;
        }

        private void BrowseTarget_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFolderDialog { Title = "Select target folder" };
            if (dlg.ShowDialog() == true) _vm.TargetDirectory = dlg.FolderName;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_vm.Name)
                || string.IsNullOrWhiteSpace(_vm.SourceDirectory)
                || string.IsNullOrWhiteSpace(_vm.TargetDirectory))
            {
                MessageBox.Show("Please fill in all fields.", "Validation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            _main.SaveJob(_vm, _isNew, _originalName);
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
            => DialogResult = false;
    }
}
