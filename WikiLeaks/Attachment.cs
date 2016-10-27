using System.Windows.Media;

namespace WikiLeaks {
    public class Attachment{
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public string Extension { get; set; }
        public ImageSource ImageSource { get; set; }
    }
}
