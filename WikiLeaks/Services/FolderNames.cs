using System;
using System.ComponentModel.Composition;
using System.IO;
using WikiLeaks.Abstract;
using WikiLeaks.Properties;

namespace WikiLeaks.Services {

    [Export(typeof(IFolderNames))]
    public class FolderNames : IFolderNames {

        string CommonApplicationData => Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

        public string CacheFolder
        {
            get
            {
                var path = Path.Combine(CommonApplicationData, "Wikileaks");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                path = Path.Combine(path, "Cache");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                path = Path.Combine(path, Settings.Default.Repository);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return path;
            }
        }

        public string DatabaseFile => Path.Combine(CacheFolder, "Wikileaks.json");
    }
}
