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

namespace WikiLeaks
{

    public class MainWindowViewModel : ViewModelBase
    {

        public MainWindowViewModel()
        {
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

        private InternetAddressList _from;

        public InternetAddressList To
        {
            get { return _to; }
            set { Set(ref _to, value); }
        }

        private InternetAddressList _to;

        public InternetAddressList Cc
        {
            get { return _cc; }
            set { Set(ref _cc, value); }
        }

        private InternetAddressList _cc;

        public string Subject
        {
            get { return _subject; }
            set { Set(ref _subject, value); }
        }

        private string _subject;

        public DateTimeOffset Date
        {
            get { return _date; }
            set { Set(ref _date, value); }
        }

        private DateTimeOffset _date;

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

        private string _htmlString;

        public bool? Validated
        {
            get { return _validated; }
            set { Set(ref _validated, value); }
        }

        private bool? _validated;

        public string MessageUrl => $"https://wikileaks.org/podesta-emails/get/{DocumentNo}";
        public string Url => $"https://wikileaks.org/podesta-emails/emailid/{DocumentNo}";

        private const string HtmlHeader = @"<meta http-equiv='Content-Type' content='text/html;charset=UTF-8'/><meta http-equiv='X-UA-Compatible' content='IE=edge'/>";


        private readonly string[] _searchTerms = new[] {"CVC", "Clinton", "Emergency", "Foundation", "HRC", "Health", "Hillary", "KSA", "Login",
            "Mills", "Obama", "Pagliano", "Password", "Podesta", "Potus", "Qatar", "Saudi", "Soros", "Striker", "Turi",
            "Urgent", "Username", "WJC" };


        private async Task RefreshPageAsync()
        {

            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                Attachments.Clear();
                HtmlString = "&nbsp;";

                MimeMessage message;

                using (var stream = await "https://wikileaks.org"
                    .AppendPathSegment("podesta-emails/get")
                    .AppendPathSegment(DocumentNo)
                    .GetStreamAsync())
                {

                    message = MimeMessage.Load(stream);
                }

                From = message.From;
                To = message.To;
                Cc = message.Cc;
                Subject = message.Subject;
                Date = message.Date;

                HtmlString = (string.IsNullOrEmpty(HtmlString)) ? message.TextBody.Replace("\r\n", "<br/>") : HtmlHeader + message.HtmlBody;

                foreach (var term in _searchTerms)
                {
                    HtmlString = HtmlString.Replace(term, HighlightName(term));
                }

                GetAttachments(message);

                Validated = new EmailValidation().ValidateSource(message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }

        public ObservableCollection<Attachment> Attachments { get; set; } = new ObservableCollection<Attachment>();

        private void GetAttachments(MimeMessage message)
        {

            foreach (var mimeEntity in message.Attachments)
            {

                var attachment = Attachment.Load(mimeEntity);

                if (attachment != null)
                {
                    Attachments.Add(attachment);
                }
            }
        }


        private static string HighlightName(string text)
        {
            return $@"<strong style=""color:#408FBF"">>{text}<</strong>";
        }
    }
}
