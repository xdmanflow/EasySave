using Microsoft.Win32;
using System.Windows;
using EasySave.GUI.ViewModels;

namespace EasySave.GUI.Views
{
    /// <summary>Code-behind for the Settings dialog.</summary>
    public partial class SettingsWindow : Window
    {
        private readonly SettingsViewModel _vm;
        private readonly MainViewModel     _main;

        public SettingsWindow(MainViewModel main)
        {
            InitializeComponent();
            _main = main;
            _vm   = new SettingsViewModel(main.GetSettings());
            DataContext = _vm;
        }

        private void BrowseCrypto_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Executable|*.exe|All files|*.*" };
            if (dlg.ShowDialog() == true) _vm.CryptoSoftPath = dlg.FileName;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _main.ApplySettings(_vm.ToModel());
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
            => DialogResult = false;
    }
}