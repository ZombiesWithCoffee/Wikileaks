﻿using System;
using System.Collections.Generic;
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
        }

        public ICommand SaveChanges => new RelayCommand(() =>{
            Settings.Default.Domain = Domain;
            Settings.Default.Repository = Repository;
            Settings.Default.Save();

            CloseAction();
        });


        public string Domain { get; set; }

        public string Repository { get; set; }

        public Action CloseAction { get; set; }

        public Dictionary<string, string> Repositories => new Dictionary<string, string>{
            {"podesta-emails",  "Podesta Emails"},
            {"dnc-emails", "DNC Emails" }
        };
    };
}
