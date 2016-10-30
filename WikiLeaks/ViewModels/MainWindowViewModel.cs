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
using WikiLeaks.Abstract;
using WikiLeaks.Dialogs;
using WikiLeaks.Enums;
using WikiLeaks.Models;
using WikiLeaks.Properties;

namespace WikiLeaks.ViewModels {

    [Export(typeof(IMainWindowViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]

    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel{

        public MainWindowViewModel(){}

        [ImportingConstructor]
        public MainWindowViewModel(IEmailValidation emailValidation, IHighlighter highlighter){
            _emailValidation = emailValidation;
            _highlighter = highlighter;
        }

        readonly IEmailValidation _emailValidation;
        readonly IHighlighter _highlighter;

        public void Initialize(){
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

        public ICommand SettingsCommand => new RelayCommand(() =>{
            var dialog = new SettingsDialog();
            dialog.ShowDialog();
        });

        public ICommand RefreshCommand => new RelayCommand(async () => await RefreshPageAsync());

        public ICommand WikileaksCommand => new RelayCommand(() =>
        {
            Process.Start(Url);
        });

        #region Properties

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

        public string HtmlString
        {
            get { return _htmlString; }
            set { Set(ref _htmlString, value); }
        }

        string _htmlString;

        public SignatureValidation SignatureValidation
        {
            get { return _validated; }
            set { Set(ref _validated, value); }
        }

        SignatureValidation _validated;

        public string MessageUrl => $"https://wikileaks.org/podesta-emails/get/{DocumentNo}";
        public string Url => $"https://wikileaks.org/podesta-emails/emailid/{DocumentNo}";

        #endregion

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

        private async Task RefreshPageAsync() {

            Mouse.OverrideCursor = Cursors.Wait;

            try{
                Clear();

                var message = await GetMimeMessage();

                From = message.From;
                To = message.To;
                Cc = message.Cc;
                Subject = message.Subject;
                Date = message.Date;

                // Format the text

                var text = string.IsNullOrEmpty(message.HtmlBody) ? message.TextBody.Replace("\r\n", "<br/>") : message.HtmlBody;

                text = _highlighter.HighlightSearchTerms(text);

                // Change the HTML to be more friendly to this WebControl

                HtmlString = @"<meta http-equiv='Content-Type' content='text/html;charset=UTF-8'/><meta http-equiv='X-UA-Compatible' content='IE=edge'/>" + text;
                GetAttachments(message);

                SignatureValidation = _emailValidation.ValidateSource(message);
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }
            finally {
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }

        async Task<MimeMessage> GetMimeMessage(){

            using (var client = new HttpClient()){
                using (var response = await client.GetAsync(new Uri(MessageUrl))){
                    var stream = await response.Content.ReadAsStreamAsync();

                    return MimeMessage.Load(stream);
                }
            }
        }

        void Clear(){
            Attachments.Clear();
            HtmlString = "&nbsp;";
            From = null;
            To = null;
            Cc = null;
            Subject = null;
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
