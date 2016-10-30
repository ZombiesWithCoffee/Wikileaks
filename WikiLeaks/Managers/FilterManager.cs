using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiLeaks.Models;

namespace WikiLeaks.Managers
{
    /*
    public class FilterManager
    {
        private MainWindow _mainWindow = null;

        public FilterManager(MainWindow wnd)
        {
            _mainWindow = wnd;
            Filters = new List<Filter>();
        }

        public FilterManager(MainWindow wnd, string filterFolder)
        {
            _mainWindow = wnd;

            if (Filters == null)
                Filters = new List<Filter>();

            Filters.Clear();

            if (string.IsNullOrWhiteSpace(filterFolder))
                return;

            try
            {
                string[] fileEntries = Directory.GetFiles(filterFolder);

                foreach (string pathToFile in fileEntries)
                {
                    if (!File.Exists(pathToFile))
                        continue;

                    string tmp = File.ReadAllText(pathToFile);
                    if (string.IsNullOrWhiteSpace(tmp))
                        continue;

                    Filter f = JsonConvert.DeserializeObject<Filter>(tmp);
                    if (f == null)
                        continue;

                    Filters.Add(f);
                }
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
                _mainWindow.UpdateUi(ex.Message, "messagebox.show");
            }

            _filterFolder = filterFolder;

        }

        string _filterFolder = "";

        public List<Filter> Filters{ get; set; }

  
        public bool AddFilter(Filter f)
        {
            if(f == null || string.IsNullOrWhiteSpace(_filterFolder))
            {
                _mainWindow.UpdateUi("Filter is null or path isn't defined.", "messagebox.show");
                return false;
            }

            string pathToFile = Path.Combine(_filterFolder, f.Name +".json");

            if (File.Exists(pathToFile))
            {
                _mainWindow.UpdateUi("Filter already exists.", "messagebox.show");
                return false;
            }

            try
            {
                using (FileStream fs = File.Create(pathToFile))
                {
                    string dataasstring = JsonConvert.SerializeObject(f);
                    byte[] info = new UTF8Encoding(true).GetBytes(dataasstring);
                    fs.Write(info, 0, info.Length);
                    byte[] data = new byte[] { 0x0 };
                    fs.Write(data, 0, data.Length);

                    Filters.Add(f);
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

        public bool SaveFilter(string originalName , Filter f )
        {
            if (f == null || string.IsNullOrWhiteSpace(_filterFolder))
            {
                _mainWindow.UpdateUi("Filter is null or path isn't defined.", "messagebox.show");
                return false;
            }

            string pathToFile = Path.Combine(_filterFolder, originalName + ".json");

            try
            {
                using (FileStream fs = File.Create(pathToFile))
                {
                    string dataasstring = JsonConvert.SerializeObject(f);
                    byte[] info = new UTF8Encoding(true).GetBytes(dataasstring);
                    fs.Write(info, 0, info.Length);
                    byte[] data = new byte[] { 0x0 };
                    fs.Write(data, 0, data.Length);
                }

                if (originalName != f.Name)
                    File.Move(Path.Combine(_filterFolder, originalName + ".json"), Path.Combine(_filterFolder, f.Name + ".json"));
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
                _mainWindow.UpdateUi(ex.Message, "messagebox.show");
                return false;
            }

            Filter update = Filters.FirstOrDefault(lf => lf.Name == originalName || lf.Name == f.Name);

            if (update != null)
                Filters[Filters.IndexOf(update)] = f;

            return true;

        }

        public bool DeleteFilter(string filterName)
        {
            if (string.IsNullOrWhiteSpace(filterName) || string.IsNullOrWhiteSpace(_filterFolder))
            {
                _mainWindow.UpdateUi("Filter is null or path isn't defined.", "messagebox.show");
                return false;
            }


            string pathToFile = Path.Combine(_filterFolder, filterName + ".json");

            if (!File.Exists(pathToFile))
            {
                return true; //return false file doesn't exist at path
            }
            try
            {
                File.Delete(pathToFile);
                Filter delete = Filters.FirstOrDefault(lf => lf.Name == filterName);
                if (delete != null)
                    Filters.RemoveAt(Filters.IndexOf(delete));
            }
            catch(Exception ex)
            {
                Debug.Assert(false, ex.Message);
                _mainWindow.UpdateUi(ex.Message, "messagebox.show");
                return false;
            }

            return true;
        }


        public Filter GetFilter(string name)
        {
            return Filters.FirstOrDefault(lf => lf.Name == name);
        }
    }
    */
}
