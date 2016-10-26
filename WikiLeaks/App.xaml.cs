using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WikiLeaks.Properties;

namespace WikiLeaks {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        void Application_Exit(object sender, ExitEventArgs e) {
            Settings.Default.Save();
        }
    }

}
