using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using MimeKit;
using WikiLeaks.Abstract;
using WikiLeaks.Extensions;

namespace WikiLeaks.Services {

    [Export(typeof(IEmailValidation))]
    public class EmailValidation : IEmailValidation{

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
}

