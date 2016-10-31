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
            if (!Settings.Default.WillHighlight) return text;

            foreach (var term in Settings.Default.SearchTerms)
            {
                text = Regex.Replace(text, $@"(?:\b{term}\b)", HighlightName(term), RegexOptions.IgnoreCase);
                // This should (in theory) not break HTML formatting but i havent figured out how to get pass the memory leak
                //text = Regex.Replace(text, $@"(?:\b{term}\b)|(?![^<]*>)", HighlightName(term), RegexOptions.IgnoreCase);
            }

            return text;
        }

        static string HighlightName(string text) {
            return $@"<span class='highlight'>{text}</span>";
        }
    }
}
