using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WikiLeaks.Properties;

namespace WikiLeaks.ViewModels {

    [Export(typeof(HighlightViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]

    public class HighlightViewModel : ViewModelBase {

        [ImportingConstructor]
        public HighlightViewModel() {

            WillHighlight = Settings.Default.WillHighlight;

            foreach (var term in Settings.Default.SearchTerms)
                SearchTerms.Add(term);
        }

        public ICommand SaveChanges => new RelayCommand(() => {
            Settings.Default.WillHighlight = WillHighlight;
            Settings.Default.SearchTerms = new StringCollection();

            foreach (var term in SearchTerms)
                Settings.Default.SearchTerms.Add(term);

            Settings.Default.Save();

            CloseAction();
        });

        public bool WillHighlight { get; set; }

        public ObservableCollection<string> SearchTerms { get; set; } = new ObservableCollection<string>();

        public string SearchTerm { get; set; }

        public string SelectedSearchTerm { get; set; }

        public ICommand AddTerm => new RelayCommand(() => {
            if (string.IsNullOrEmpty(SearchTerm))
                return;

            SearchTerms.Add(SearchTerm);
        });

        public ICommand RemoveTerm => new RelayCommand(() => {
            if (string.IsNullOrEmpty(SelectedSearchTerm))
                return;

            SearchTerms.Remove(SelectedSearchTerm);
        });

        public ICommand ResetTerms => new RelayCommand(() =>
        {
            SearchTerms.Clear();

            foreach (var term in Settings.Default.DefaultTerms)
                SearchTerms.Add(term);
        });

        public Action CloseAction { get; set; }
    };
}
