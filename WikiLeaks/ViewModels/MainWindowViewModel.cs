using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
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

        public MainWindowViewModel(){
        }

        [ImportingConstructor]
        public MainWindowViewModel(IEmailValidation emailValidation, IHighlighter highlighter, IEmailCache emailCache, IAttachmentHistory attachmentHistory){
            _emailValidation = emailValidation;
            _highlighter = highlighter;
            _emailCache = emailCache;
            _attachmentHistory = attachmentHistory;

            Documents = _attachmentHistory.Initialize();

            RaisePropertyChanged(nameof(Documents));
        }

        void DownloadTimer(){
            
            _timer = new DispatcherTimer{
                Interval = new TimeSpan(1000)
            };

            _timer.Tick += (sender, args) => {
                DocumentNo++;
            };

//            _timer.Start();
        }

        DispatcherTimer _timer;

        readonly IEmailValidation _emailValidation;
        readonly IEmailCache _emailCache;
        readonly IHighlighter _highlighter;
        readonly IAttachmentHistory _attachmentHistory;

        public List<Document> Documents { get; set; }

        public Document SelectedDocument
        {
            get { return _selectedDocument; }
            set
            {
                Set(ref _selectedDocument, value);

                DocumentNo = value.DocumentId;
                TabIndex = 1;
                RaisePropertyChanged(nameof(TabIndex));
            }
        }

        Document _selectedDocument;

        public int TabIndex { get; set; }

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

        public ICommand SettingsCommand => new RelayCommand(async() =>{
            new SettingsDialog().ShowDialog();

            await RefreshPageAsync();
        });

        public ICommand UpdateDatabaseCommand => new RelayCommand(async () =>{
            Documents = await _attachmentHistory.RefreshAsync();
        });

        public ICommand HighlightCommand => new RelayCommand(async() => {
            new HighlightDialog().ShowDialog();
            await RefreshPageAsync();
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

        public string Url => $"https://wikileaks.org/{Settings.Default.Repository}/emailid/{DocumentNo}";

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

                var message = await _emailCache.GetMimeMessageAsync(DocumentNo);

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
