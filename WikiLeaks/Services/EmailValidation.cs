using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using MimeKit;
using MimeKit.Cryptography;
using WikiLeaks.Abstract;
using WikiLeaks.Enums;
using WikiLeaks.Extensions;

namespace WikiLeaks.Services {

    [Export(typeof(IEmailValidation))]
    public class EmailValidation : IEmailValidation{

        [ImportingConstructor]
        public EmailValidation(IDkimPublicKeyLocator dkimPublicKeyLocator){
            _dkimPublicKeyLocator = dkimPublicKeyLocator;
        }

        readonly IDkimPublicKeyLocator _dkimPublicKeyLocator;

        public SignatureValidation ValidateSource(MimeMessage message){

            var dkim = message.GetDkimHeader();

            if (dkim == null)
                return SignatureValidation.NoSignature;

            try{
                var validation = message.Verify(dkim, _dkimPublicKeyLocator);

                return validation ? SignatureValidation.Valid : SignatureValidation.Invalid;
            }
            catch(Exception ex){
                Debug.WriteLine(ex.Message);
                return SignatureValidation.NoPublicKey;
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

