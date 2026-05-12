using System.Windows;
using EasySave.GUI.ViewModels;
using EasySave.GUI.Views;
using EasySave.Languages;

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
                ApplyLanguage();
            };

            ApplyLanguage();
        }

        private void ApplyLanguage()
        {
            var lang = new LanguageManager(_vm.GetSettings().Language);

            TxtTotalJobs.Text = lang.Get("gui_total_jobs");
            TxtStatus.Text = lang.Get("gui_status");
            TxtQuickActions.Text = lang.Get("gui_quick_actions");
            BtnRunAll.Content = lang.Get("gui_run_all");
            BtnSettings.Content = lang.Get("gui_settings");
            TxtBackupJobs.Text = lang.Get("gui_backup_jobs");
            BtnAdd.Content = lang.Get("gui_add");
            BtnEdit.Content = lang.Get("gui_edit");
            BtnDelete.Content = lang.Get("gui_delete");
            BtnRun.Content = lang.Get("gui_run");
            ColName.Header = lang.Get("gui_col_name");
            ColSource.Header = lang.Get("gui_col_source");
            ColTarget.Header = lang.Get("gui_col_target");
            ColType.Header = lang.Get("gui_col_type");
        }
    }
}