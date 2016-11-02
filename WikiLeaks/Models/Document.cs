using System;

namespace WikiLeaks.Models
{
    public class Document
    {
        public int DocumentId { get; set; }

        public DateTimeOffset DateTime { get; set; }

        public string Subject { get; set; }

        public string From{ get; set; }
    }
}
