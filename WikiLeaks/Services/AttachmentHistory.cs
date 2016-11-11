using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WikiLeaks.Abstract;
using WikiLeaks.Models;

namespace WikiLeaks.Services {


    [Export(typeof(IAttachmentHistory))]
    public class AttachmentHistory : IAttachmentHistory{

        [ImportingConstructor]
        public AttachmentHistory(IFolderNames folderNames, IEmailCache emailCache){
            _folderNames = folderNames;
            _emailCache = emailCache;
        }

        readonly IFolderNames _folderNames;
        readonly IEmailCache _emailCache;

        public List<Document> Initialize(){

            if(! File.Exists(_folderNames.DatabaseFile))
                return new List<Document>();

            var json = File.ReadAllText(_folderNames.DatabaseFile);

            try{
                return JsonConvert.DeserializeObject<List<Document>>(json);
            }
            catch{
                return new List<Document>();
            }
        }

        public async Task RefreshAsync(List<Document> documents){

            try{
                for (var documentNo = 50888; documentNo <= 59258; documentNo++){

                    try{
                        var message = await _emailCache.GetMimeMessageAsync(documentNo);

                        if (message == null)
                            continue;

                        var document = new Document{
                            DocumentId = documentNo,
                            DateTime = message.Date,
                            Subject = message.Subject,
                            From = message.From.FirstOrDefault()?.Name
                        };

                        documents.Add(document);
                    }
                    catch{
                        continue;
                    }
                }

                var json = JsonConvert.SerializeObject(documents, Formatting.Indented);

                File.WriteAllText(_folderNames.DatabaseFile, json);
            }
            catch (Exception ex){
                return;
            }
        }
    }
}
