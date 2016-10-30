using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiLeaks.Extensions
{
    /*
    public static class HtmlEx
    {
        public static string FixHtml(this string html)
        {
            if (string.IsNullOrEmpty(html))
                return html;

            html = html.Replace("\t", "");
            html = html.Replace("\n", @"<br/>");
            html = html.Replace("&lt;", "<");
            html = html.Replace("&gt;", ">");
            return html.ToString();
            
        }

        /// <summary>
        /// TODO implement options like exactMatch = t/f. startsWith, contains, endsWith
        /// </summary>
        /// <param name="html"></param>
        /// <param name="searchTerm"></param>
        /// <param name="color"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string HighlightText(this string html, string searchTerm, string color, string options ="")
        {
            StringBuilder tmp = new StringBuilder(html);
            tmp = tmp.Replace(searchTerm, $@"<strong style=""color:{color}"">{searchTerm}</strong>");
            tmp = tmp.Replace(searchTerm.ToUpper(), $@"<strong style=""color:{color}"">{searchTerm.ToUpper()}</strong>");
            tmp = tmp.Replace(searchTerm.ToLower(), $@"<strong style=""color:{color}"">{searchTerm.ToLower()}</strong>");
            return tmp.ToString();
        }

        public static string GetHtmlHexColor(this string colorName)
        {
            Color colorValue = ColorTranslator.FromHtml(colorName);

            string hexColor= String.Format("#{0:X2}{1:X2}{2:X2}", colorValue.R,
                                                     colorValue.G,
                                                     colorValue.B);

            return hexColor;

        }
    }
    */
}
