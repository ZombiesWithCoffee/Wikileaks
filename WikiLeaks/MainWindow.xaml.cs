using System.ComponentModel.Composition;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using mshtml;
using WikiLeaks.Models;
using WikiLeaks.Managers;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Threading;
using WikiLeaks.Abstract;
using WikiLeaks.Models;

namespace WikiLeaks {

    [Export(typeof(MainWindow))]
    public partial class MainWindow : IPartImportsSatisfiedNotification {

        private AppManager _application = null;
        private FilterManager _filterManager = null;
        private DocumentManager _documentManager = null;

        private Thread _searchThread = null;

        public MainWindow() {
            InitializeComponent();

            _application = new AppManager(this, "");

            if (_application.Settings == null)
                return;

            this.txtLeakStartIdSetting.Text = _application.Settings.SearchStartId.ToString();
            this.txtLeakEndIdSetting.Text = _application.Settings.SearchEndId.ToString();
            this.txtLeakEndId.Text = this.txtLeakEndIdSetting.Text;
            this.txtLeakStartId.Text = this.txtLeakStartIdSetting.Text;
            this.txtBaseUrlSetting.Text = _application.Settings.BaseUrl;
            this.txtCachFolder.Text = _application.Settings.CacheFolder;
            this.txtSearchResultsFolder.Text = _application.Settings.ResultsFolder;

            _filterManager = new FilterManager(this, _application.Settings.FilterFolder);
            LoadFilters();
            _documentManager = new DocumentManager(this, _application, _filterManager);

            rdoFilterView.IsChecked = true;

            //todo attachments
            rdoAttachmentsView.Focusable = false;
            rdoAttachmentsView.IsHitTestVisible = false;
            rdoAttachmentsView.Background = new SolidColorBrush(Colors.LightGray);
            
        }

        [Import]
        public IMainWindowViewModel ViewModel
        {
            get { return DataContext as IMainWindowViewModel; }
            set { DataContext = value; }
        }

        [Import]
        public ICssStyle CssStyle { get; set; }

        public void OnImportsSatisfied() {
            ViewModel.Initialize();
        }

        void Attachment_Click(object sender, RoutedEventArgs e) {
            var button = sender as Button;
            var attachment = button?.DataContext as Attachment;

            if (attachment == null)
                return;

            var tempFileName = Path.Combine(Path.GetTempPath(), attachment.FileName);
            File.WriteAllBytes(tempFileName, attachment.Data);

            System.Diagnostics.Process.Start(tempFileName);
        }

        void WebBrowser_OnLoadCompleted(object sender, NavigationEventArgs e){
            var webBrowser = sender as WebBrowser;

            var document = webBrowser?.Document as HTMLDocument;

            if (document == null)
                return;

            CssStyle.Update(document);
        }

        #region App Settings Tab Events/Code
        private void btnSaveAppSettings_Click(object sender, RoutedEventArgs e)
        {
            _application.Settings.BaseUrl = this.txtBaseUrlSetting.Text.Trim();
            int i = 0;
            if (int.TryParse(this.txtLeakStartIdSetting.Text, out i))
                _application.Settings.SearchStartId = i;

            if (int.TryParse(this.txtLeakEndIdSetting.Text, out i))
                _application.Settings.SearchEndId = i;

            _application.Settings.CacheFolder = this.txtCachFolder.Text.Trim();

            _application.Settings.ResultsFolder = this.txtSearchResultsFolder.Text.Trim();

            _application.SaveSettings("");
        }
        #endregion

        #region Filter Tab Events/Code
        public void AutoSizeColumns()
        {
            GridView gv = lstFilters.View as GridView;
            if (gv != null)
            {
                foreach (var c in gv.Columns)
                {
                    // Code below was found in GridViewColumnHeader.OnGripperDoubleClicked() event handler (using Reflector)
                    // i.e. it is the same code that is executed when the gripper is double clicked
                    if (double.IsNaN(c.Width))
                    {
                        c.Width = c.ActualWidth;
                    }
                    c.Width = double.NaN;
                }
            }
        }


        private void btnAddFilter_Click(object sender, RoutedEventArgs e)
        {
            Filter f = new Filter();

            f.Name = txtFilterName.Text.Trim();
            int tmp = -1;
            if (int.TryParse(txtFilterPriority.Text.Trim(), out tmp))
                f.Priority = tmp;

            f.SearchTokens = txtFilterSearchTokens.Text.Trim();
            f.HighlightColor = txtFilterHighlightColor.Text.Trim();
            try
            {
               System.Drawing.Color myColor = ColorTranslator.FromHtml(f.HighlightColor);
            }
            catch (Exception ex)
            {
                MessageBox.Show("You must supply a valie html color. " + ex.Message);
                return;
            }

            if (_filterManager.AddFilter(f))
            {
                LoadFilters();
            }
        }

        private void btnSaveFilter_Click(object sender, RoutedEventArgs e)
        {
            Filter f = new Filter();
            int priority = 0;
            int.TryParse(txtFilterPriority.Text.Trim(), out priority);
            f.Priority = priority;

            f.Name = txtFilterName.Text.Trim();

            if (string.IsNullOrWhiteSpace(f.Name))
            {
                MessageBox.Show("You must supply a name.");
                txtFilterName.Focus();
                return;
            }
            f.SearchTokens = txtFilterSearchTokens.Text.Trim();
            f.HighlightColor = txtFilterHighlightColor.Text.Trim();

            try
            {
                System.Drawing.Color myColor = ColorTranslator.FromHtml(f.HighlightColor);
            }
            catch (Exception ex)
            {
                MessageBox.Show("You must supply a valid html color. " + ex.Message);
                return;
            }

            Filter selected = (Filter)lstFilters.SelectedItem;

            string originalName = f.Name;
            if (selected != null && selected.Name != f.Name)
                originalName = selected.Name;

            if (_filterManager.SaveFilter(originalName, f))
                LoadFilters();
        }

        private void btnDeleteFilter_Click(object sender, RoutedEventArgs e)
        {
            Filter f = (Filter)lstFilters.SelectedItem;
            if (f == null)
            {
                return;
            }

            if (_filterManager.DeleteFilter(f.Name))
            {
                LoadFilters();
            }
        }

        private void lstFilters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtFilterName.Text = "";
            txtFilterPriority.Text = "";
            txtFilterSearchTokens.Text = "";
            txtFilterHighlightColor.Text = "";

            Filter f = (Filter)lstFilters.SelectedItem;
            if (f == null)
            {
                return;
            }

            txtFilterName.Text = f.Name;
            txtFilterPriority.Text = f.Priority.ToString();
            txtFilterSearchTokens.Text = f.SearchTokens;
            txtFilterHighlightColor.Text = f.HighlightColor;
        }

        #endregion

        #region Search Results Events/Code

        private string _viewState = "FilterView"; //default to this.

    
        private void rdoFilterView_Click(object sender, RoutedEventArgs e)
        {
            _viewState = "FilterView";

            if (_filterManager.Filters == null)
                return;

            List<Filter> sortedFilters = _filterManager.Filters.OrderBy(ob => ob.Priority).ToList();
            tvwSearchResults.Items.Clear();

            //For now make the root nodes based on the filter name.
            //If we loaded by leak id it could take a while.
            //Later we can do an xref to find what document id's match
            //multiple filters. 
            //
            foreach (Filter f in sortedFilters)
            {
                TreeViewItem node = new TreeViewItem();
                node.Header = f.Name;
                node.Tag = f.Name; //note: this could be used to hold the object.
                tvwSearchResults.Items.Add(node);
            }
        }

        private void rdoResultsView_Click(object sender, RoutedEventArgs e)
        {
            _viewState = "ResultsView";

            tvwSearchResults.Items.Clear();

            //show files from results folder
            // 
            string[] fileEntries = Directory.GetFiles(_application.Settings.ResultsFolder, "*.html", System.IO.SearchOption.AllDirectories);

            foreach (string pathToFile in fileEntries)
            {
                string file = Path.GetFileNameWithoutExtension(pathToFile);
                TreeViewItem node = new TreeViewItem();
                node.Header = file;
                node.Tag = file;
                tvwSearchResults.Items.Add(node);
            }

        }

        private void rdoCacheView_Click(object sender, RoutedEventArgs e)
        {
            _viewState = "CacheView";

            //show files from cache
            tvwSearchResults.Items.Clear();

            //show files from results folder
            // 
            string[] fileEntries = Directory.GetFiles(_application.Settings.CacheFolder, "*.html", System.IO.SearchOption.AllDirectories);

            foreach (string pathToFile in fileEntries)
            {
                string file = Path.GetFileNameWithoutExtension(pathToFile);
                TreeViewItem node = new TreeViewItem();
                node.Header = file;
                node.Tag = file;
                tvwSearchResults.Items.Add(node);
            }
        }

        private void rdoAttachmentsView_Click(object sender, RoutedEventArgs e)
        {
            _viewState = "AttachmentsView";
            tvwSearchResults.Items.Clear();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            int startId = -1;
            int.TryParse(txtLeakStartId.Text.Trim(), out startId);

            int endId = -1;
            int.TryParse(txtLeakEndId.Text.Trim(), out endId);

            if (startId <= 0 || endId < startId)
            {
                MessageBox.Show("You must provide valid start and end ids!");
                return;
            }

            //default to the filter view because it adds to the view as the docs are
            //processed. 
            rdoFilterView.IsChecked = true;
           
            LoadFilters();
            // SearchDocuments(startId, endId); //sync call for debuging
            _searchThread = new Thread(() => SearchDocuments(startId, endId));
            _searchThread.Start();
        }

        private void btnCancelSearch_Click(object sender, RoutedEventArgs e)
        {
            _documentManager.CancelSearch = true;

            _searchThread.Join(TimeSpan.FromSeconds(15));
        }

        private void SearchDocuments(int startId, int endId)
        {
            ToggleRadioButtonsForSearch(false);

            //todo put a flag in the filter class to set as active.
            //this way we can toggle from the ui then save to file
            //Then filter here where Filter.Enabled == true;
            //
            foreach (Filter filter in _filterManager.Filters)
            {
                _documentManager.SearchDocuments(startId, endId,  filter.Name, false);
            }

            ToggleRadioButtonsForSearch(true);
            MessageBox.Show("Document search completed.");
        }

        private void ToggleRadioButtonsForSearch(bool enableState)
        {
            System.Windows.Media.Color c = Colors.LightGray;

            if(enableState)
                c = Colors.White;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                rdoResultsView.Focusable           = enableState;
                rdoResultsView.IsHitTestVisible    = enableState;
                rdoCacheView.Focusable             = enableState;
                rdoCacheView.IsHitTestVisible = enableState;

                //todo attachments
                rdoAttachmentsView.Focusable        = false;
                rdoAttachmentsView.IsHitTestVisible = false;
                rdoAttachmentsView.Background = new SolidColorBrush(Colors.LightGray);
            }));

        }

        private void tvwSearchResults_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem tvi = null;

            if (e.NewValue is TreeViewItem)
                tvi = (TreeViewItem)e.NewValue;

            if (tvi == null)
                return;

            string nodeName = tvi.Tag?.ToString();

            string viewDirectory = _application.Settings.ResultsFolder;

            switch (_viewState)
            {
                case "FilterView":
                case "ResultsView":
                    viewDirectory = _application.Settings.ResultsFolder;
                    break;
                case "CacheView":
                    viewDirectory = _application.Settings.CacheFolder;
                    break;
                case "AttachmentsView":
                    viewDirectory = _application.Settings.CacheFolder + "Attachments\\";
                    break;
            }

            float ftmp;
            if (_viewState != "AttachmentsView" &&  float.TryParse(nodeName, out ftmp))
            {
                if ( (ftmp % 1) != 0 )
                    return; //todo  launch app for the attachement or open folder with file selected

                string pathToFile = string.Format("{0}{1}.html", viewDirectory, nodeName);

                if (!File.Exists(pathToFile))
                {
                    MessageBox.Show( "FILE NOT FOUND! " + pathToFile);
                    return;
                }
                //show the email
                this.WebBrowser.Navigate(pathToFile);
                Dispatcher.BeginInvoke((Action)(() => tabControl.SelectedIndex = 0));
                return;
            }
            //else load filter

        }
        #endregion

        //Used in DocumentManager to update the status of the ui as it processes.
        //
        public void UpdateUi(object resultInfo, string uiElement){

            switch (uiElement.ToLower())
            {
                case "treeview.results":
                    SearchResult tvInfo = (SearchResult)resultInfo;
                    //Because another thread is sending this info to update
                    //the ui we have to let the dispatching/owner thread update the ui
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action<SearchResult>(UpdateResultTree), tvInfo);
                    break;
                case "progressbar.update":

                    break;
                case "messagebox.show":
                    MessageBox.Show(resultInfo?.ToString());
                    break;
                case "radiobutton.toggle":
                    ToggleRadioButtonsForSearch((bool)resultInfo);
                    break;
            }
        }

        private void UpdateResultTree(SearchResult tvInfo)
        {
            //find the root node
            TreeViewItem parentNode = null;
            int parentIndex = 0;
            foreach (TreeViewItem rootNode in tvwSearchResults.Items)
            {
                if ((rootNode.Tag.ToString() == tvInfo.FilterName))
                {
                    rootNode.Header = tvInfo.FilterName + " (*)"; //" (" + tvInfo.ResultCount.ToString() + ")";
                    parentNode = rootNode;
                    break;
                }
                parentIndex++;
            }

            if (parentNode == null)
                return;
            //find the child node under the parent
            foreach (TreeViewItem childNode in parentNode.Items)
            {
                if ((childNode.Tag.ToString() == tvInfo.LeakId.ToString()))
                {
                    return;//found, already here so bail out.
                }
            }
            int count = 0;
            tvInfo.LeakHitCount.TryGetValue(tvInfo.LeakId, out count);

            TreeViewItem node = new TreeViewItem();
            node.Header = tvInfo.LeakId.ToString() + " (*)"; // " (" + count.ToString() + ")";
            node.Tag = tvInfo.LeakId.ToString();
            parentNode.Items.Add(node);
        }

        //Loads the treeview in results area and the listview in filters tab.
        //
        private void LoadFilters()
        {
            if (_filterManager.Filters == null)
                return;

            List<Filter> sortedFilters = _filterManager.Filters.OrderBy(ob => ob.Priority).ToList();

            lstFilters.ItemsSource = null;
            lstFilters.ItemsSource = sortedFilters;
            AutoSizeColumns();

            tvwSearchResults.Items.Clear();

            //For now make the root nodes based on the filter name.
            //If we loaded by leak id it could take a while.
            //Later we can do an xref to find what document id's match
            //multiple filters. 
            //
            foreach (Filter f in sortedFilters)
            {
                TreeViewItem node = new TreeViewItem();
                node.Header = f.Name;
                node.Tag = f.Name; //note: this could be used to hold the object.
                tvwSearchResults.Items.Add(node);
            }
        }

    
    }
}
