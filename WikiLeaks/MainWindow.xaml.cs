using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using mshtml;

namespace WikiLeaks
{

    public partial class MainWindow
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindowViewModel ViewModel
        {
            get { return DataContext as MainWindowViewModel; }
            set { DataContext = value; }
        }

        private void Attachment_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var attachment = button?.DataContext as Attachment;

            if (attachment == null)
                return;

            var tempFileName = Path.Combine(Path.GetTempPath(), Path.ChangeExtension(attachment.FileName, attachment.Extension));
            File.WriteAllBytes(tempFileName, attachment.Data);

            System.Diagnostics.Process.Start(tempFileName);
        }

        private void WebBrowser_OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            var webBrowser = sender as WebBrowser;

            if (webBrowser == null)
            {
                return;
            }

            var document = ((WebBrowser)sender).Document as HTMLDocument;

            if (document == null)
                return;

            var styleSheet = document.createStyleSheet("", 0);

            styleSheet.cssText = @"
                h2   {color: blue;}

                #content header {
                    color: #000;
                    border-bottom: 1px solid #000;
                    border-top: 5px solid #000;
                    padding-bottom: 5px;
                }
            ";
        }
    }
}
