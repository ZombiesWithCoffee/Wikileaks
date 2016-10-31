using System.ComponentModel.Composition;
using System.Text.RegularExpressions;
using WikiLeaks.Abstract;
using WikiLeaks.Properties;
using WikiLeaks.ViewModels;

namespace WikiLeaks.Services {

    [Export(typeof(IHighlighter))]
    public class ClassHighlighter : IHighlighter{

      

        public string HighlightSearchTerms(string text)
        {
            foreach (var term in Settings.Default.SearchTerms)
                text = Regex.Replace(text, $@"{term}", HighlightName(term), RegexOptions.IgnoreCase);

            return text;
        }

        static string HighlightName(string text) {
            return $@"<span class=""highlight"">{text}</span>";
        }
    }
}
