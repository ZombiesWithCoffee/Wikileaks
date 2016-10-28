using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MimeKit;
using WikiLeaks.Properties;

namespace WikiLeaks{

    public class Attachment{
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public string Extension { get; set; }
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

                    case "image/gif":{
                        var imageSource = new BitmapImage();
                        imageSource.BeginInit();
                        imageSource.StreamSource = memory;
                        imageSource.EndInit();

                        // Assign the Source property of your image
                        attachment.ImageSource = imageSource;
                        attachment.Extension = "gif";
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

                    case "application/msword":
                        attachment.ImageSource = BitmapToImageSource(Resources.Microsoft_Word);
                        attachment.Extension = "doc";
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
