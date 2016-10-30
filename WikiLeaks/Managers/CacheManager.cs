using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiLeaks.Extensions;

namespace WikiLeaks.Managers
{
    public  class CacheManager
    {
        private MainWindow _mainWindow = null;
        string _cacheFolder = EnvironmentEx.AppDataFolder;

        public bool CacheIdsLoaded { get; set; }
        //Made this a float so we can save attachment files as
        //the decimal portion in the order they are loaded.
        //example email id = 25001 has two attachments (hypothetically).
        //25001.1.<fileext> would be first attachment
        //25001.2.<fileext> would e second attachment.
        //May have to figure out where ( in \attachments sub-folder?) to store non text attachments.
        //put reference data in json file for these non text attachments?
        //
        public List<float>  CacheIds { get; set; }

        public CacheManager(MainWindow wnd, string pathToFolder)
        {
            _mainWindow = wnd;

            CacheIds = new List<float>();

            CacheIdsLoaded = false;
            if (!string.IsNullOrWhiteSpace(pathToFolder) && Directory.Exists(pathToFolder))
                _cacheFolder = pathToFolder;

            //load ids only so we don't bog down ram full of documents if all were doing is a cache hit check
            LoadCache();
        }

        public void LoadCache(bool idsOnly= true)
        {
            CacheIds.Clear();
            
            string[] fileEntries = Directory.GetFiles(_cacheFolder,"*.*", System.IO.SearchOption.AllDirectories);

            foreach (string pathToFile in fileEntries)
            {
               string fileRoot = Path.GetFileNameWithoutExtension(pathToFile);
               float fileId;

               if(float.TryParse(fileRoot,out fileId ))
                    CacheIds.Add(fileId);
            }
            CacheIdsLoaded = true;
        }

        public bool IsCached(float leakId)
        {
            if(!CacheIdsLoaded)
                LoadCache();

            return CacheIds.Where(w => w == leakId).Count() > 0 ? true : false;
        }

        public string GetCachedFile(float leakId)
        {
            string pathToFile = _cacheFolder;

            if ((leakId % 1) != 0)
            {
                pathToFile += "Attachments\\";
            }

            pathToFile += leakId.ToString() + ".html";

            if (!File.Exists(pathToFile))
                return "";

            string doc = File.ReadAllText(pathToFile);
            return doc;
        }


        public bool SaveToCache(float leakId ,string documentBody)
        {
            string pathToFile = _cacheFolder;

            if ((leakId % 1) != 0)
            {
                pathToFile += "Attachments\\";
            }

            pathToFile += leakId.ToString() + ".html";

            try
            {
                File.WriteAllText(pathToFile, documentBody);
                CacheIds.Add(leakId);
            }
            catch(Exception ex)
            {
                Debug.Assert(false, ex.Message);
                _mainWindow.UpdateUi(ex.Message, "messagebox.show");
                return false;
            }
            return true;
        }
    }
}
