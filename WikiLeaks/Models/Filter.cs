using System.Collections.Generic;


namespace WikiLeaks.Models
{
    public class Filter
    {
        public string Name { get; set; }

        //grep,application
        public string Type { get; set; }

        public string HighlightColor { get; set; }

        public string SearchTokens { get; set; }

        public int Priority { get; set; }
    }
}
