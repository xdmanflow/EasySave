using System.Windows;
using EasySave.GUI.ViewModels;
using EasySave.GUI.Views;

namespace EasySave.GUI
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();
            _vm = new MainViewModel();
            DataContext = _vm;

            _vm.RequestEditJob += vm =>
            {
                var win = new JobEditWindow(vm, _vm) { Owner = this };
                win.ShowDialog();
            };

            _vm.RequestOpenSettings += () =>
            {
                var win = new SettingsWindow(_vm) { Owner = this };
                win.ShowDialog();
            };
        }
    }
}