using System;
using System.ComponentModel.Composition;
using System.IO;
using WikiLeaks.Abstract;

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

                return path;
            }
        }
    }
}
