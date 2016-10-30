using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiLeaks.Extensions
{
    public class EnvironmentEx
    {
        public static string AppDataFolder
        {
            get
            {
                //Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "default.appsettings.json");
                string tmp = Environment.CurrentDirectory.Replace("bin\\Debug", "").Replace("bin\\Release", "");
                return tmp + "App_Data";
            }
        }

    }
}
