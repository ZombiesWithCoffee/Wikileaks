using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WikiLeaks.Properties;

namespace WikiLeaks.ViewModels {

    [Export(typeof(FiltersViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]

    public class FiltersViewModel : ViewModelBase {

        [ImportingConstructor]
        public FiltersViewModel() {
        }

        public ICommand SaveChanges => new RelayCommand(() =>{
            Settings.Default.Save();

            CloseAction();
        });

        public string Domain { get; set; }

        public Action CloseAction { get; set; }
    };
}
