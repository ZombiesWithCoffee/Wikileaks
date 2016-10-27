using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using MimeKit;
using MimeKit.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using WikiLeaks.Properties;

namespace WikiLeaks {
    public class EmailValidation {

        public bool? ValidateSource(string text){

            text = RepairHtml(text);

            using (var stream = GenerateStreamFromString(text)) {
                var message = MimeMessage.Load(stream);
                var dkim = message.GetDkimHeader();

                if (dkim == null)
                    return null;

                var dkimLocator = new DkimLocator();

                try{
                    return message.Verify(dkim, dkimLocator);
                }
                catch(Exception ex){
                    return null;
                }
            }
        }

        /// <summary>
        /// This part gets weird.  In the HTML section of the attachment, there are some strange artifacts.
        /// I think Wikileaks is incorrectly formatting some of it when they display it.  We'll have to work around it or
        /// the signature won't validate
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>

        public string RepairHtml(string text){

            var startPos = text.IndexOf(@"Content-Type: text/html; charset=UTF-8", StringComparison.Ordinal);

            if (startPos == -1)
                return text;

            var formatted = new StringBuilder();
            formatted.Append(text.Substring(0, startPos));

            for (int index = startPos; index < text.Length; index++) {
                switch (text[index]) {
                    case '\'':
                        formatted.Append("&#39;");
                        break;

                    case '<':
                        if (text[index + 1] == '<') {
                            formatted.Append("&lt;<");
                            index++;
                        }
                        else {
                            formatted.Append(text[index]);
                        }
                        break;

                    case '>':
                        if (text[index + 1] == '>') {
                            formatted.Append(">&gt;");
                            index++;
                        }
                        else {
                            formatted.Append(text[index]);
                        }
                        break;

                    default:
                        formatted.Append(text[index]);
                        break;
                }
            }

            return formatted.ToString();
        }

        public static Stream GenerateStreamFromString(string s) {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }

    public class DkimLocator : IDkimPublicKeyLocator {

        public AsymmetricKeyParameter LocatePublicKey(string methods, string domain, string selector, CancellationToken cancellationToken = new CancellationToken()){

            var dnsLookup = $"{selector}._domainkey.{domain}";

            switch (dnsLookup){
                case "google._domainkey.hillaryclinton.com":
                    return PublicKeyFactory.CreateKey(Convert.FromBase64String(Resources.HillaryClinton));

                case "20120113._domainkey.gmail.com":
                    return PublicKeyFactory.CreateKey(Convert.FromBase64String(Resources.Gmail20120113));

                case "q20140121._domainkey.comcast.net":
                    return PublicKeyFactory.CreateKey(Convert.FromBase64String(Resources.ComcastQ20140121));

                default:
                    return null;
            }
        }

        public static Stream GenerateStreamFromString(string s) {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }

    public static class MimeKitHelpers {
        public static Header GetDkimHeader(this MimeMessage msg) {
            return msg.Headers.FirstOrDefault(header => header.Id == HeaderId.DkimSignature);
        }
    }
}

