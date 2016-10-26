using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace WikiLeaks {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        public MainWindowViewModel ViewModel
        {
            get { return DataContext as MainWindowViewModel; }
            set { DataContext = value; }
        }

        private void Attachment_Click(object sender, RoutedEventArgs e) {
            var button = sender as Button;
            var attachment = button?.DataContext as Attachment;

            if (attachment != null){
                var url = $@"https://wikileaks.org/{attachment.Href}";
                System.Diagnostics.Process.Start(url);
            }
        }

        private void WebBrowser_OnLoadCompleted(object sender, NavigationEventArgs e){
            var webBrowser = sender as WebBrowser;
            if (webBrowser == null) {
                return;
            }
        }
    }
}
