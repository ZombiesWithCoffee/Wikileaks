using System.ComponentModel.Composition;
using WikiLeaks.ViewModels;

namespace WikiLeaks.Dialogs {

    /// <summary>
    /// Interaction logic for FiltersDialog.xaml
    /// </summary>

    public partial class HighlightDialog {

        public HighlightDialog() {
            InitializeComponent();

            App.Container?.SatisfyImportsOnce(this);

            ViewModel.CloseAction = Close;
        }

        [Import]
        public HighlightViewModel ViewModel
        {
            get { return DataContext as HighlightViewModel; }
            set { DataContext = value; }
        }
    }
}
