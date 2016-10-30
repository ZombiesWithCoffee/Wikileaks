using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using WikiLeaks.Extensions;
using WikiLeaks.Models;

namespace WikiLeaks.Managers
{
    /*
    public class AppManager
    {
        public AppSetting Settings { get; set; }

        private MainWindow _mainWindow;

        public AppManager(MainWindow wnd, string pathToFile)
        {
            _mainWindow = wnd;

            Settings = new AppSetting();

            if (string.IsNullOrWhiteSpace(pathToFile) || !File.Exists(pathToFile))
            {
                pathToFile = Path.Combine(EnvironmentEx.AppDataFolder, "default.appsettings.json");

                if (!File.Exists(pathToFile)&& CreateDefaultFile(pathToFile) ==false)
                {
                    _mainWindow.UpdateUi("FAILED TO GET APP SETTINGS FILE!", "messagebox.show");
                    return;
                }
            }

            try
            {
                string tmp = File.ReadAllText(pathToFile);
                if (string.IsNullOrWhiteSpace(tmp))
                {
                    CreateDefaultFile(pathToFile);
                    tmp = File.ReadAllText(pathToFile);
                }
                Settings = JsonConvert.DeserializeObject<AppSetting>(tmp);
            }
            catch(Exception ex)
            {
                Debug.Assert(false, ex.Message);
                _mainWindow.UpdateUi(ex.Message, "messagebox.show");
            }

        }


        protected bool CreateDefaultFile(string pathToFile)
        {
            try
            {
                using (FileStream fs = File.Create(pathToFile))
                {
                    Settings.SearchStartId = 1;
                    Settings.SearchEndId = 20;
                    Settings.BaseUrl = "https://wikileaks.org/podesta-emails/emaildid/";
                    Settings.CacheFolder = Path.Combine( EnvironmentEx.AppDataFolder, "Cache\\");

                    Settings.FilterFolder = Path.Combine(EnvironmentEx.AppDataFolder, "Filters\\");

                    string dataasstring = JsonConvert.SerializeObject(Settings);
                    byte[] info = new UTF8Encoding(true).GetBytes(dataasstring);
                    fs.Write(info, 0, info.Length);
                    byte[] data = new byte[] { 0x0 };
                    fs.Write(data, 0, data.Length);
                }
            }
            catch(Exception ex)
            {
                Debug.Assert(false, ex.Message);
                _mainWindow.UpdateUi(ex.Message, "messagebox.show");
                return false;
            }
            return true;
        }

        public bool SaveSettings(string pathToFile)
        {
            if(string.IsNullOrWhiteSpace(pathToFile))
                pathToFile = Path.Combine(EnvironmentEx.AppDataFolder, "default.appsettings.json");
            
            try
            {
                using (var fs = File.Create(pathToFile))
                {
                    var dataasstring = JsonConvert.SerializeObject(Settings);
                    var info = new UTF8Encoding(true).GetBytes(dataasstring);
                    fs.Write(info, 0, info.Length);
                    var data = new byte[] { 0x0 };
                    fs.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
                _mainWindow.UpdateUi(ex.Message, "messagebox.show");
                return false;
            }
            return true;
        }
    }
    */
}
