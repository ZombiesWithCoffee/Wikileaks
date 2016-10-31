using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WikiLeaks.Models;
using WikiLeaks.Properties;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace WikiLeaks.ViewModels {

    [Export(typeof(HighlightViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]

    public class HighlightViewModel : ViewModelBase {

        [ImportingConstructor]
        public HighlightViewModel() {
        }

        public ICommand SaveChanges => new RelayCommand(() =>{
            Settings.Default.Save();

            CloseAction();
        });

        [Editor(typeof(CollectionEditor), typeof(CollectionEditor))]
        public ObservableCollection<Highlight> Highlights { get; set; } = new ObservableCollection<Highlight>();

        public Action CloseAction { get; set; }
    };
}
