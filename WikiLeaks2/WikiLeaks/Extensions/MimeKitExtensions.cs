using System.Linq;
using MimeKit;

namespace WikiLeaks.Extensions {

    public static class MimeKitExtensions {
        public static Header GetDkimHeader(this MimeMessage msg) {
            return msg.Headers.FirstOrDefault(header => header.Id == HeaderId.DkimSignature);
        }
    }
}
