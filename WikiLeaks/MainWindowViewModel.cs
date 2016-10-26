using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

            var html = new StringBuilder($@"<meta http-equiv='Content-Type' content='text/html;charset=UTF-8'/>{text}");

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

            html = html.Replace("WJC",     HighlightColor("WJC"));
            html = html.Replace("HRC",     HighlightColor("HRC"));
            html = html.Replace("Obama", HighlightColor("Obama"));
            html = html.Replace("Hillary", HighlightColor("Hillary"));
            html = html.Replace("Clinton", HighlightColor("Clinton"));
            html = html.Replace("Mills", HighlightColor("Mills"));
            html = html.Replace("podesta", HighlightColor("podesta"));
            html = html.Replace("Podesta", HighlightColor("Podesta"));
            html = html.Replace("Soros", HighlightColor("Soros", "FF0000"));
            html = html.Replace("SOROS", HighlightColor("SOROS", "FF0000"));

            html = html.Replace("Pagliano", HighlightColor("Pagliano", "FF0000"));
            html = html.Replace("PAGLIANO", HighlightColor("PAGLIANO", "FF0000"));
           
        }

        string HighlightColor(string text, string color = "408FBF") {
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
