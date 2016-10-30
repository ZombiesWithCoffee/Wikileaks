using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiLeaks.Models
{
    public class SearchResult
    {
        public string FilterName { get; set; }

        //total count for the filter (sum of all search term hits).
        public int ResultCount { get; set; }


        //string is search term.
        //int is cound
        //keeps track if search term was found in doc. how effective the search term is..
           public Dictionary<string, int> SearchTermHitCount = new Dictionary<string, int>();

        //float is leak id
        //int is count
        //this is to keep track of how often a search term was found (not how many were found in doc).
        public Dictionary<float, int> LeakHitCount = new Dictionary<float, int>();

        public float LeakId { get; set; }

        public string Document { get; set; }

    }
}
