using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MimeKit;
using WikiLeaks.Models;
using WikiLeaks.Properties;
using WikiLeaks.Services;

namespace WikiLeaks.ViewModels {

    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel{

        [ImportingConstructor]
        public MainWindowViewModel() 
        {
            DocumentNo = Settings.Default.DocumentNo;
        }

        public ICommand NextCommand => new RelayCommand(() =>
        {
            DocumentNo++;
        });

        public ICommand PreviousCommand => new RelayCommand(() =>
        {
            DocumentNo--;
        });

        public ICommand RefreshCommand => new RelayCommand(async () => await RefreshPageAsync());

        public ICommand WikileaksCommand => new RelayCommand(() =>
        {
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

        private readonly string[] _searchTerms = {"CVC", "Clinton", "Emergency", "Foundation", "HRC", "Health", "Hillary", "KSA", "Login",
            "Mills", "Obama", "Pagliano", "Password", "Podesta", "Potus", "Qatar", "Saudi", "Soros", "Striker", "Turi",
            "Urgent", "Username", "WJC" };

        private async Task RefreshPageAsync() {

            Mouse.OverrideCursor = Cursors.Wait;

            try {
                Attachments.Clear();
                HtmlString = "&nbsp;";

                MimeMessage message;

                using (var client = new HttpClient()) {
                    using (var response = await client.GetAsync(new Uri(MessageUrl))) {
                        var stream = await response.Content.ReadAsStreamAsync();

                        message = MimeMessage.Load(stream);
                    }
                }

                From = message.From;
                To = message.To;
                Cc = message.Cc;
                Subject = message.Subject;
                Date = message.Date;

                var text = string.IsNullOrEmpty(message.HtmlBody) ? message.TextBody.Replace("\r\n", "<br/>") : message.HtmlBody;

                foreach (var term in _searchTerms)
                    text = text.Replace(term, HighlightName(term));

                HtmlString = @"<meta http-equiv='Content-Type' content='text/html;charset=UTF-8'/><meta http-equiv='X-UA-Compatible' content='IE=edge'/>" + text;
                GetAttachments(message);

                Validated = new EmailValidation().ValidateSource(message);
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }
            finally {
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }

        static string HighlightName(string text) {
            return $@"<strong style=""color:#408FBF"">>{text}<</strong>";
        }

        public ObservableCollection<Attachment> Attachments { get; set; } = new ObservableCollection<Attachment>();

        void GetAttachments(MimeMessage message) {

            foreach (var mimeEntity in message.BodyParts) {

                var attachment = Attachment.Load(mimeEntity);

                if (attachment != null) {
                    Attachments.Add(attachment);
                }
            }
        }

    }
}
