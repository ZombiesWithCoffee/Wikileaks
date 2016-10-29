using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiLeaks.Models
{
    public class Document
    {
        public int LeakId { get; set; }

        public string LeakUrl { get; set; }

        public string Body { get; set; }

        public bool IsAttachment { get; set; }

        public string Type { get; set; }

        public DateTime DocDate { get; set; }
    }
}
