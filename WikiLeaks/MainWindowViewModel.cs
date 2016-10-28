using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Flurl;
using Flurl.Http;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MimeKit;
using WikiLeaks.Properties;

namespace WikiLeaks {

    public class MainWindowViewModel : ViewModelBase {

        public MainWindowViewModel() {
            DocumentNo = Settings.Default.DocumentNo;
        }

        public ICommand NextCommand => new RelayCommand(() => {
            DocumentNo++;
        });

        public ICommand PreviousCommand => new RelayCommand(() => {
            DocumentNo--;
        });

        public ICommand RefreshCommand => new RelayCommand(async () => await RefreshPageAsync());

        public ICommand WikileaksCommand => new RelayCommand(() => {
            Process.Start(Url);
        });

        public InternetAddressList From
        {
            get { return _from; }
            set { Set(ref _from, value); }
        }

        InternetAddressList _from;

        public InternetAddressList To
        {
            get { return _to; }
            set { Set(ref _to, value); }
        }

        InternetAddressList _to;

        public InternetAddressList Cc
        {
            get { return _cc; }
            set { Set(ref _cc, value); }
        }

        InternetAddressList _cc;

        public string Subject
        {
            get { return _subject; }
            set { Set(ref _subject, value); }
        }

        string _subject;

        public DateTimeOffset Date
        {
            get { return _date; }
            set { Set(ref _date, value); }
        }

        DateTimeOffset _date;

        public int DocumentNo
        {
            get { return Settings.Default.DocumentNo; }
            set
            {
                Settings.Default.DocumentNo = value;
                Settings.Default.Save();

                RaisePropertyChanged();
                // ReSharper disable once ExplicitCallerInfoArgument
                RaisePropertyChanged(nameof(Url));

                RefreshPageAsync();
            }
        }

        public string HtmlString
        {
            get { return _htmlString; }
            set { Set(ref _htmlString, value); }
        }

        string _htmlString;

        public bool? Validated
        {
            get { return _validated; }
            set { Set(ref _validated, value); }
        }

        bool? _validated;

        public string MessageUrl => $"https://wikileaks.org/podesta-emails/get/{DocumentNo}";
        public string Url => $"https://wikileaks.org/podesta-emails/emailid/{DocumentNo}";

        async Task RefreshPageAsync() {

            Mouse.OverrideCursor = Cursors.Wait;

            try {
                Attachments.Clear();
                HtmlString = "&nbsp;";

                MimeMessage message;

                using (var stream = await "https://wikileaks.org"
                    .AppendPathSegment("podesta-emails/get")
                    .AppendPathSegment(DocumentNo)
                    .GetStreamAsync()) {

                    message = MimeMessage.Load(stream);
                }

                From = message.From;
                To = message.To;
                Cc = message.Cc;
                Subject = message.Subject;
                Date = message.Date;

                HtmlString = message.HtmlBody;

                if (string.IsNullOrEmpty(HtmlString)){

                    var text = message.TextBody;
                    text = text.Replace("\r\n", "<br/>");

                    HtmlString = text;
                }
                else{
                    HtmlString = @"<meta http-equiv='Content-Type' content='text/html;charset=UTF-8'/><meta http-equiv='X-UA-Compatible' content='IE=edge'/>" + HtmlString;
                }

                // HighlightNames(ref html);
                GetAttachments(message);

                Validated = new EmailValidation().ValidateSource(message);
            }
            catch (Exception ex) {
                int j = 0;
            }
            finally {
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }

        public ObservableCollection<Attachment> Attachments { get; set; } = new ObservableCollection<Attachment>();

        void GetAttachments(MimeMessage message) {

            foreach (var mimeEntity in message.Attachments) {

                var attachment = Attachment.Load(mimeEntity);

                if(attachment != null) { 
                    Attachments.Add(attachment);
                }
            }
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

        void HighlightName(ref StringBuilder html, string name, string color = "408FBF") {

            html = html.Replace(name, HighlightName(name, color));
            html = html.Replace(name.ToUpper(), HighlightName(name.ToUpper(), color));
            html = html.Replace(name.ToLower(), HighlightName(name.ToLower(), color));
        }

        string HighlightName(string text, string color) {
            return $@"<strong style=""color:#{color}"">{text}</strong>";
        }
    }
}
