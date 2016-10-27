using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Flurl;
using Flurl.Http;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MimeKit;
using WikiLeaks.Properties;

namespace WikiLeaks {

    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel {

        public MainWindowViewModel() {
            DocumentNo = Settings.Default.DocumentNo;
        }

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
                // ReSharper disable once ExplicitCallerInfoArgument
                RaisePropertyChanged(nameof(Url));

                RefreshPage();
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

        private bool? _validated;

        async Task RefreshPage() {

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

        void GetAttachments(MimeMessage message) {

            foreach (var mimeEntity in message.Attachments) {

                using (var memory = new MemoryStream()){
                    var mimePart = mimeEntity as MimePart;

                    if (mimePart != null){
                        mimePart.ContentObject.DecodeTo(memory);

                        var attachment = new Attachment{
                            Data = memory.GetBuffer(),
                            FileName = mimePart.FileName
                        };

                        switch (mimePart.ContentType.MimeType){

                            case "image/png":{
                                var imageSource = new BitmapImage();
                                imageSource.BeginInit();
                                imageSource.StreamSource = memory;
                                imageSource.EndInit();

                                // Assign the Source property of your image
                                attachment.ImageSource = imageSource;
                                attachment.Extension = "png";
                                break;
                            }

                            case "image/jpeg":{

                                var image = Image.FromStream(memory);
                                var bitmap = new Bitmap(image);

                                attachment.ImageSource = BitmapToImageSource(bitmap);
                                attachment.Extension = "jpg";
                                break;
                            }

                            case "application/ics":
                                attachment.ImageSource = BitmapToImageSource(Resources.Calendar);
                                attachment.Extension = "ics";
                                break;

                            case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                                attachment.ImageSource = BitmapToImageSource(Resources.Microsoft_Word);
                                attachment.Extension = "docx";
                                break;

                            case "application/pdf":
                                attachment.FileName = mimePart.FileName;
                                attachment.ImageSource = BitmapToImageSource(Resources.PDF);
                                attachment.Extension = "pdf";
                                break;

                            case "APPLICATION/octet-stream":
                                attachment.FileName = mimePart.FileName;
                                attachment.Extension = "dat";
                                attachment.ImageSource = BitmapToImageSource(Resources.PDF);
                                break;

                            default:
                                attachment.FileName = mimePart.FileName;
                                attachment.ImageSource = BitmapToImageSource(Resources.PDF);
                                break;
                        }

                        Attachments.Add(attachment);
                    }
                }
            }
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap) {
            using (var memory = new MemoryStream()) {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;

                var bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        public ObservableCollection<Attachment> Attachments { get; set; } = new ObservableCollection<Attachment>();

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

        public string MessageUrl => $"https://wikileaks.org/podesta-emails/get/{DocumentNo}";
        public string Url => $"https://wikileaks.org/podesta-emails/emailid/{DocumentNo}";

        public ICommand NextCommand => new RelayCommand(() =>
        {
            DocumentNo++;
        });

        public ICommand PreviousCommand => new RelayCommand(() =>
        {
            DocumentNo--;
        });

        public ICommand UpdateCommand => new RelayCommand(async () => await RefreshPage());

        public ICommand RefreshCommand => new RelayCommand(async () => await RefreshPage());

        public ICommand WikileaksCommand => new RelayCommand(() =>
        {
            System.Diagnostics.Process.Start(Url);
        });
    }
}
