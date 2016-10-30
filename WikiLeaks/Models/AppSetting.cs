using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//string filePath = SystemEx.GetConfigValue(Globals.ReleaseState + ".EmailNewMember", "~/App_Data/Templates/Site/EmailNewMember.html");
//filePath = HostingEnvironment.MapPath(filePath);
//if (!File.Exists(filePath))
//{
//    //SetStatus(true, "Email template file EmailOrderReceived.html is missing.");
//    return false;
//}
//string txtBody = File.ReadAllText(filePath);

namespace WikiLeaks.Models
{
    public class AppSetting
    {
        public int SearchStartId { get; set; }

        public int SearchEndId { get; set; }

        public string BaseUrl { get; set; }

        public string CacheFolder { get; set; }

        public string FilterFolder { get; set; }

        public string ResultsFolder { get; set; }
    }
}
