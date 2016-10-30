using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WikiLeaks.Extensions;
using WikiLeaks.Models;

namespace WikiLeaks.Managers
{
    /*
    public class DocumentManager
    {
        private AppManager _application = null;
        private FilterManager _filterManager = null;
        private CacheManager _cacheManager = null;
        private MainWindow _mainWindow = null;
        private readonly Regex _attachmentRegex = new Regex("</span>(?<FileName>.+)<br><small>(?<FileSize>.+)<br>(?<ImageType>.+)</small>");

        public bool CancelSearch{ get; set; }
        private Dictionary<float, SearchResult> _searchResults = new Dictionary<float, SearchResult>();

        //Just being lazy, don't feel like loading all the settings..
        //
        public DocumentManager(MainWindow wnd, AppManager appManager, FilterManager filterManager)
        {
            CancelSearch = false;

            if (wnd == null || appManager == null || filterManager == null)
            {
                Debug.Assert(false, "OBJECT IS NULL");
            }

            _mainWindow = wnd;
            _application = appManager;
            _filterManager = filterManager;

            _cacheManager = new CacheManager(_mainWindow, _application.Settings.CacheFolder);
         
        }

        public void SearchDocuments(int startId, int endId,  string filterName, bool includeAttachements)
        {
            if (startId > endId || startId < 0)
            {
                _mainWindow.UpdateUi("INVALID IDs", "messagebox.show");
                return;
            }

            if (string.IsNullOrWhiteSpace(filterName))
            {
                _mainWindow.UpdateUi("INVALID FILTER", "messagebox.show");
                return;
            }

            Filter searchFilter =   _filterManager.Filters.Where(wf => wf.Name?.ToUpper() == filterName.ToUpper()).FirstOrDefault();

            if(searchFilter == null)
            {
                _mainWindow.UpdateUi("FILTER WASN'T FOUND", "messagebox.show");
                return;
            }
            //Iterate through the cache ID's because they contain references to attachments
            //
            List<float> cacheIds = _cacheManager.CacheIds.Where(w => w == startId && w <= endId).ToList();

            for (float i = startId; i <= endId; i++)
            {
               if(!cacheIds.Contains(i))
                     cacheIds.Add(i);
            }

            foreach (float cacheId in cacheIds)
            {
                if (CancelSearch)
                {
                    CancelSearch = false;
                    return;
                }

                SearchResult searchResult = new SearchResult();
                searchResult.FilterName = filterName;
                searchResult.ResultCount = 0;
                searchResult.LeakId = cacheId;

                if (_searchResults.ContainsKey(cacheId))
                {   //Is it loaded? We've already processed this doc for another search term, re-use it.
                    searchResult = _searchResults[cacheId];

                    if (searchResult.FilterName != filterName)
                        searchResult.ResultCount = 0;

                    searchResult.FilterName = filterName;
                    //searchResult.SearchTermHitCount.Clear();
                    //searchResult.LeakHitCount.Clear();
                }
                else if (_cacheManager.IsCached(cacheId))
                {   //Is it local?
                    searchResult.Document = _cacheManager.GetCachedFile(cacheId);
                    searchResult.FilterName = filterName;
                    searchResult.ResultCount = 0;
                    searchResult.LeakId = cacheId;
                    _searchResults.Add(cacheId, searchResult);
                }
                else
                {   //Go get it.
                    searchResult.Document = DownloadDocument(cacheId, includeAttachements);
                    searchResult.FilterName = filterName;
                    searchResult.ResultCount = 0;
                    searchResult.LeakId = cacheId;
                    _searchResults.Add(cacheId, searchResult);
                }

                string[] searchTerms = searchFilter.SearchTokens.Split(',');
                string highlightColor = searchFilter.HighlightColor.GetHtmlHexColor();

                foreach (string searchTerm in searchTerms)
                {
                    if (!searchResult.Document.Contains(searchTerm))
                        continue;
                 
                    //update search term metrics
                    if (!searchResult.SearchTermHitCount.ContainsKey(searchTerm.ToLower()))
                        searchResult.SearchTermHitCount.Add(searchTerm.ToLower(), 1);
                    else
                        searchResult.SearchTermHitCount[searchTerm.ToLower()] += 1;

                    //update document hits metrics
                    if (!searchResult.LeakHitCount.ContainsKey(cacheId))
                        searchResult.LeakHitCount.Add(cacheId, 1);
                    else
                        searchResult.LeakHitCount[cacheId] += 1;

                    searchResult.ResultCount++;
                
                    searchResult.Document = searchResult.Document.HighlightText(searchTerm, highlightColor);

                    _searchResults[cacheId] = searchResult;

                    SaveToResults(cacheId, searchResult.Document);

                    if (CancelSearch)
                    {
                        CancelSearch = false;
                        return;
                    }
                }

                ////this will update the treeview, by default it shows the filters in the
                ////root, so if the filter found something in an email then add/update
                ////the email id under the filter node.
                if (searchResult.ResultCount > 0 && _mainWindow != null)
                {
                    _mainWindow.UpdateUi(searchResult, "treeview.results");
                }
            }
        }

        protected bool SaveToResults(float leakId, string document)
        {
            string pathToFile = _application.Settings.ResultsFolder + leakId + ".html";

            try
            {
                File.WriteAllText(pathToFile, document);
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
                _mainWindow.UpdateUi(ex.Message, "messagebox.show");
                return false;
            }
            return true;

        }

        //html agility pack doesn't have an async function.
        //  public async Task<string> DownloadDocument(float leakId, bool ignoreCache = false)
        public string DownloadDocument(float leakId, bool includeAttachments, bool ignoreCache = false)
        {
            if (_cacheManager.IsCached(leakId) && ignoreCache)
            {
               return _cacheManager.GetCachedFile(leakId);
            }

            //Since this is going in the wild I'm sure they don't want every neckbeard on earth slamming their servers.
            // 10 second delay between requests.
            //LOOK FOR ALREADY DOWNLOADED CACHE FILES! DON'T BE A DICK! 
            Thread.Sleep(TimeSpan.FromSeconds(5));

            string url = _application.Settings.BaseUrl;
            var web = new HtmlWeb();
           //  bool downloadAttachment = false;
             

            //TODO this is where it gets tricky cause they don't have a fractional representation of their
            //attachements, so I'll have to research how the attachements are represented.
            //if ((leakId % 1) != 0)
            //{
            //    url += Math.Floor(leakId).ToString();
            //    downloadAttachment = true; //the leak is an attachment
            //}

            url  += leakId.ToString();

             var document = web.Load(url);
            
            var node = document.DocumentNode.SelectSingleNode("//div[@id='content']");

            if (node == null)
            {
                return document.DocumentNode.InnerHtml; ;
            }

            #region   Get attachments BOOKMARK LATEST I was implementing this but wanted to check in first.
            //if (downloadAttachment || includeAttachments)
            //{
            //    var attachmentNode = document.DocumentNode.SelectNodes("//div[@id='attachments']//a");

            //    if (attachmentNode == null)
            //        return "";

            //    foreach (var attachment in document.DocumentNode.SelectNodes("//div[@id='attachments']//a")){

            //        //var match = _attachmentRegex.Match(attachment.InnerHtml);

            //        //    if (match.Success){
            //        //        Attachments.Add(new Attachment{
            //        //            FileName = match.Groups["FileName"].Value,
            //        //            FileSize = match.Groups["FileSize"].Value,
            //        //            Href = attachment.Attributes["href"].Value,
            //        //            ImageType = match.Groups["ImageType"].Value
            //        //        });
            //        //    }
            //        //    else{
            //        //        Attachments.Add(new Attachment{
            //        //            Href = attachment.Attributes["href"].Value
            //        //        });
            //        //    }
            //    }

            //}
                #endregion

           var text = @"<div id='content'>" + node.InnerHtml.TrimStart('\n', '\t');

            text = text.Replace("\n\t\t\t\t\n\t\t\t\t<header>", "<header>");
            text = text.Replace("</h2>\n\t\t\t\t", "</h2>");
            text = text.Replace("</header>\n\n\n\n\t\t\t\t", "</header>");
            text = text.Replace("sh\">\n\t\t\t\t\t\t\n\t\t\t\t\t\t", "sh\">");

            string html = $@"<meta http-equiv='Content-Type' content='text/html;charset=UTF-8'/><meta http-equiv='X-UA-Compatible' content='IE=edge'/>{text}".FixHtml();
            _cacheManager.SaveToCache(leakId, html);
            return html;
        }


        public string GetResultFile(float leakId) {

            if (_searchResults.ContainsKey(leakId))
                return _searchResults[leakId].Document;

            string pathToFile = _application.Settings.ResultsFolder;

            if ((leakId % 1) != 0)
            {
                // pathToFile += "Attachments\\";
                return "TODO IMPLEMENT ATTACHEMENTS";
            }

            pathToFile += leakId.ToString() + ".html";

            if (!File.Exists(pathToFile))
                return "File not found.";

            return File.ReadAllText(pathToFile);
           
        }
    }
    */
}
