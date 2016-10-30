using System.ComponentModel.Composition;
using WikiLeaks.ViewModels;

namespace WikiLeaks.Dialogs {

    /// <summary>
    /// Interaction logic for FiltersDialog.xaml
    /// </summary>

    public partial class FiltersDialog {

        public FiltersDialog() {
            InitializeComponent();

            App.Container?.SatisfyImportsOnce(this);

            ViewModel.CloseAction = Close;
        }

        [Import]
        public FiltersViewModel ViewModel
        {
            get { return DataContext as FiltersViewModel; }
            set { DataContext = value; }
        }
    }
}
