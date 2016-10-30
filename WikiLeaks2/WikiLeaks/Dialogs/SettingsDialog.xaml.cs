using System.Windows;
using System.ComponentModel.Composition;
using WikiLeaks.ViewModels;

namespace WikiLeaks.Dialogs {

    /// <summary>
    /// Interaction logic for PrintCardDialog.xaml
    /// </summary>

    public partial class SettingsDialog {

        public SettingsDialog() {
            InitializeComponent();

            App.Container?.SatisfyImportsOnce(this);

            ViewModel.CloseAction = Close;
        }

        [Import]
        public SettingsViewModel ViewModel
        {
            get { return DataContext as SettingsViewModel; }
            set { DataContext = value; }
        }
    }
}
