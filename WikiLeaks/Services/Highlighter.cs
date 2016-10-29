using System.ComponentModel.Composition;

namespace WikiLeaks.Services {

 //   [Export(typeof(IHighlighter))]
    public class Highlighter : IHighlighter{

        readonly string[] _searchTerms = {"CVC", "Clinton", "Emergency", "Foundation", "HRC", "Health", "Hillary", "KSA", "Login",
            "Mills", "Obama", "Pagliano", "Password", "Podesta", "Potus", "Qatar", "Saudi", "Soros", "Striker", "Turi",
            "Urgent", "Username", "WJC" };

        public string HighlightSearchTerms(string text){
            foreach (var term in _searchTerms)
                text = text.Replace(term, HighlightName(term));

            return text;
        }

        static string HighlightName(string text) {
            return $@"<strong style=""color:#408FBF"">>{text}<</strong>";
        }
    }
}
