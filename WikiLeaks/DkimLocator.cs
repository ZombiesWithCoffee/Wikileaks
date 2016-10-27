using System;
using System.IO;
using System.Net;
using System.Threading;
using MimeKit.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using WikiLeaks.Properties;

namespace WikiLeaks {

    public class DkimLocator : IDkimPublicKeyLocator {

        public AsymmetricKeyParameter LocatePublicKey(string methods, string domain, string selector, CancellationToken cancellationToken = new CancellationToken()) {

            var dnsLookup = $"{selector}._domainkey.{domain}";

            switch (dnsLookup) {
                case "google._domainkey.hillaryclinton.com":
                    return PublicKeyFactory.CreateKey(Convert.FromBase64String(Resources.HillaryClinton));

                case "20120113._domainkey.gmail.com":
                    return PublicKeyFactory.CreateKey(Convert.FromBase64String(Resources.Gmail20120113));

                case "q20140121._domainkey.comcast.net":
                    return PublicKeyFactory.CreateKey(Convert.FromBase64String(Resources.ComcastQ20140121));

                case "dccckey._domainkey.dccc.org":
                    return PublicKeyFactory.CreateKey(Convert.FromBase64String(Resources.DcccKey));

                case "1000073432._domainkey.auth.ccsend.com":
                    return PublicKeyFactory.CreateKey(Convert.FromBase64String(Resources.CcSend1000073432));

                case "20120113._domainkey.google.com":
                    return PublicKeyFactory.CreateKey(Convert.FromBase64String(Resources.Google20120113));

                case "bsdkey._domainkey.bounce.bluestatedigital.com":
                    return PublicKeyFactory.CreateKey(Convert.FromBase64String(Resources.BlueStateDigital));
                    
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
}
