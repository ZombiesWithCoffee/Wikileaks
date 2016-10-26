using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HtmlAgilityPack;
using WikiLeaks.Properties;

namespace WikiLeaks {

    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel{

        public MainWindowViewModel(){
            DocumentNo = Settings.Default.DocumentNo;
        }

        public string Website
        {
            get { return _website; }
            set { Set(ref _website, value); }
        }

        string _website;

        public int DocumentNo
        {
            get { return Settings.Default.DocumentNo; }
            set
            {
                Settings.Default.DocumentNo = value;
                Settings.Default.Save();

                RaisePropertyChanged();
                RefreshPage(); 
                // ReSharper disable once ExplicitCallerInfoArgument
                RaisePropertyChanged(nameof(Url));
            }
        }

        public string Repository
        {
            get { return _repository; }
            set { Set(ref _repository, value); }
        }

        string _repository;

        public string Action
        {
            get { return _action; }
            set { Set(ref _action, value); }
        }

        string _action;

        public string HtmlString
        {
            get { return _htmlString; }
            set { Set(ref _htmlString, value); }
        }

        private string _htmlString;

        public ICommand UpdateCommand => new RelayCommand(RefreshPage);

        void RefreshPage(){

            Attachments.Clear();
            HtmlString = "&nbsp;";

            var web = new HtmlWeb();
            var document = web.Load(Url);

            var node = document.DocumentNode.SelectSingleNode("//div[@id='content']");

            if (node == null)
                return;

            var text = node.InnerHtml.TrimStart('\n', '\t');

            var html = new StringBuilder($@"<meta http-equiv='Content-Type' content='text/html;charset=UTF-8'/><meta http-equiv='X-UA-Compatible' content='IE=edge'/>{text}");

            FixHtml(ref html);
            HighlightNames(ref html);

            HtmlString = html.ToString();

            // Get attachments

            var attachmentNode = document.DocumentNode.SelectNodes("//div[@id='attachments']//a");

            if (attachmentNode == null)
                return;

            foreach (var attachment in document.DocumentNode.SelectNodes("//div[@id='attachments']//a")){

                var match = _attachmentRegex.Match(attachment.InnerHtml);

                if (match.Success){
                    Attachments.Add(new Attachment{
                        FileName = match.Groups["FileName"].Value,
                        FileSize = match.Groups["FileSize"].Value,
                        Href = attachment.Attributes["href"].Value,
                        ImageType = match.Groups["ImageType"].Value
                    });
                }
                else{
                    Attachments.Add(new Attachment{
                        Href = attachment.Attributes["href"].Value
                    });
                }
            }
        }

        readonly Regex _attachmentRegex = new Regex("</span>(?<FileName>.+)<br><small>(?<FileSize>.+)<br>(?<ImageType>.+)</small>");

        public ObservableCollection<Attachment> Attachments { get; set; } = new ObservableCollection<Attachment>();

        void FixHtml(ref StringBuilder html){
            html = html.Replace("\t", "");
            html = html.Replace("\n", @"<br/>");
            html = html.Replace("&lt;", "<");
            html = html.Replace("&gt;", ">");
        }

        void HighlightNames(ref StringBuilder html) {
            HighlightName(ref html, "WJC");
            HighlightName(ref html, "HRC");
            HighlightName(ref html, "KSA");
            HighlightName(ref html, "CVC");
            HighlightName(ref html, "Obama");
            HighlightName(ref html, "Hillary");
            HighlightName(ref html, "Clinton");
            HighlightName(ref html, "Mills");
            HighlightName(ref html, "Podesta");
            HighlightName(ref html, "Soros", "FF0000");
            HighlightName(ref html, "Turi");
            HighlightName(ref html, "Qatar");
            HighlightName(ref html, "Striker");
            HighlightName(ref html, "Saudi");
            HighlightName(ref html, "Foundation");
            HighlightName(ref html, "Pagliano");
            HighlightName(ref html, "Login");
            HighlightName(ref html, "Password");
            HighlightName(ref html, "Username");
            HighlightName(ref html, "Health");
            HighlightName(ref html, "Emergency");
            HighlightName(ref html, "Urgent");
            HighlightName(ref html, "Potus");
        }

        void HighlightName(ref StringBuilder html, string name, string color = "408FBF"){

            html = html.Replace(name, HighlightName(name, color));
            html = html.Replace(name.ToUpper(), HighlightName(name.ToUpper(), color));
            html = html.Replace(name.ToLower(), HighlightName(name.ToLower(), color));
        }

        string HighlightName(string text, string color) {
            return $@"<strong style=""color:#{color}"">{text}</strong>";
        }

        public string Url => $"https://wikileaks.org/podesta-emails/emailid/{DocumentNo}";

        public ICommand NextCommand => new RelayCommand(() => {
            DocumentNo++;
        });

        public ICommand PreviousCommand => new RelayCommand(() => {
            DocumentNo--;
        });

        public ICommand RefreshCommand => new RelayCommand(RefreshPage);

        public ICommand WikileaksCommand => new RelayCommand(() => {
            System.Diagnostics.Process.Start(Url);
        });
    }
}
