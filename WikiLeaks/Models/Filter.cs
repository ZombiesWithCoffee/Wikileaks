using System.Drawing;

namespace WikiLeaks.Models
{
    public class Filter
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public Color Color { get; set; }

        public string SearchTokens { get; set; }

        public int Priority { get; set; }
    }
}
