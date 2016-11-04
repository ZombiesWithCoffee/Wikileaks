using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using MimeKit;
using WikiLeaks.Abstract;

namespace WikiLeaks.Services {

    [Export(typeof(IEmailCache))]
    public class EmailCache : IEmailCache {

        [ImportingConstructor]
        public EmailCache(IFolderNames folderNames, IEmailOnline emailOnline) {
            _folderNames = folderNames;
            _emailOnline = emailOnline;
        }

        readonly IFolderNames _folderNames;
        readonly IEmailOnline _emailOnline;

        public void Delete(int documentNo){

            var fileName = GetFileName(documentNo);

            if (File.Exists(fileName)){
                File.Delete(fileName);
            }
        }

        public async Task<MimeMessage> GetMimeMessageAsync(int documentNo){

            var fileName = GetFileName(documentNo);

            if (File.Exists(fileName)){
                using (var stream = File.OpenRead(fileName)) {
                    return MimeMessage.Load(stream);
                }
            }

            var mimeMessage = await _emailOnline.GetMimeMessageAsync(documentNo);

            using (var stream = File.OpenWrite(fileName)){
                mimeMessage.WriteTo(stream);
            }

            return mimeMessage;
        }

        string GetFileName(int documentNo){
            return Path.Combine(_folderNames.CacheFolder, $"{documentNo}.eml");
        }
    }
}
