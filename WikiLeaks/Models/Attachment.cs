using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MimeKit;
using WikiLeaks.Properties;

namespace WikiLeaks.Models{

    public class Attachment{
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public ImageSource ImageSource { get; set; }

        public static Attachment Load(MimeEntity mimeEntity){

            var mimePart = mimeEntity as MimePart;

            if (mimePart == null)
                return null;

            using (var memory = new MemoryStream()){

                mimePart.ContentObject.DecodeTo(memory);

                var attachment = new Attachment{
                    Data = memory.GetBuffer(),
                    FileName = mimePart.FileName
                };

                switch (mimePart.ContentType.MimeType){

                    case "image/png":
                    case "image/gif":
                    case "image/jpg":
                    case "image/jpeg":
                    case "image/pjpeg":
                        {

                        var image = Image.FromStream(memory);
                        var bitmap = new Bitmap(image);

                        attachment.ImageSource = BitmapToImageSource(bitmap);
                        break;
                    }

                    case "application/ics":
                        attachment.ImageSource = BitmapToImageSource(Resources.Calendar);
                        break;

                    case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                        attachment.ImageSource = BitmapToImageSource(Resources.Microsoft_Word);
                        break;

                    case "application/msword":
                        attachment.ImageSource = BitmapToImageSource(Resources.Microsoft_Word);
                        break;

                    case "application/vnd.ms-excel":
                        attachment.ImageSource = BitmapToImageSource(Resources.Excel);
                        break;

                    case "application/pdf":
                        attachment.ImageSource = BitmapToImageSource(Resources.PDF);
                        break;

                    case "APPLICATION/octet-stream":
                        attachment.ImageSource = BitmapToImageSource(Resources.DAT);
                        break;

                    case "audio/x-m4a":
                        attachment.ImageSource = BitmapToImageSource(Resources.M4A);
                        break;

                    case "text/plain":
                    case "text/html":
                    case "text/calendar":
                    case "message/delivery-status":
                        return null;

                    case "video/x-ms-wmv":
                        attachment.ImageSource = BitmapToImageSource(Resources.Video);
                        break;

                    case "application/octet-stream":
                        attachment.ImageSource = BitmapToImageSource(Resources.Unknown);
                        break;

                    default:
                        attachment.ImageSource = BitmapToImageSource(Resources.Unknown);
                        break;
                }

                return attachment;
            }
        }

        static BitmapImage BitmapToImageSource(Image bitmap) {
            using (var memory = new MemoryStream()) {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;

                var bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
