using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WikiLeaks.Properties;

namespace WikiLeaks.ViewModels {

    [Export(typeof(SettingsViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]

    public class SettingsViewModel : ViewModelBase {

        [ImportingConstructor]
        public SettingsViewModel() {

            Domain = Settings.Default.Domain;
            Repository = Settings.Default.Repository;
            StartId = Settings.Default.StartId;
            EndId = Settings.Default.EndId;

            foreach (var term in Settings.Default.SearchTerms)
                SearchTerms.Add(term);
        }

        public ICommand SaveChanges => new RelayCommand(() =>{
            Settings.Default.Domain = Domain;
            Settings.Default.Repository = Repository;
            Settings.Default.StartId = StartId;
            Settings.Default.EndId = EndId;

            Settings.Default.SearchTerms = new StringCollection();
            foreach (var term in SearchTerms)
                Settings.Default.SearchTerms.Add(term);

            Settings.Default.Save();

            CloseAction();
        });

        public string Domain { get; set; }

        public string Repository { get; set; }

        public int StartId { get; set; }

        public int EndId { get; set; }

        public Action CloseAction { get; set; }

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

        public ObservableCollection<string> SearchTerms { get; set; } = new ObservableCollection<string>();

        public string SearchTerm { get; set; }

        public string SelectedSearchTerm { get; set; }

        public Dictionary<string, string> Repositories => new Dictionary<string, string>{
            {"podesta-emails",  "Podesta Emails"}
        };
    };
}
