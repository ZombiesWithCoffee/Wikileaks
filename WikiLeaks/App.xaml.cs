using System.Windows;
using WikiLeaks.Properties;

namespace WikiLeaks {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App {
        void Application_Exit(object sender, ExitEventArgs e) {
            Settings.Default.Save();
        }
    }

}
