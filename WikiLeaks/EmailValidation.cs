using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MimeKit;

namespace WikiLeaks {

    public class EmailValidation {

        public bool? ValidateSource(MimeMessage message){

            var dkim = message.GetDkimHeader();

            if (dkim == null)
                return null;

            var dkimLocator = new DkimLocator();

            try{
                return message.Verify(dkim, dkimLocator);
            }
            catch(Exception ex){
                Debug.WriteLine(ex.Message);
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

