using System.ComponentModel.Composition;
using mshtml;
using WikiLeaks.Abstract;

namespace WikiLeaks.Services {

    [Export(typeof(ICssStyle))]
    public class CssStyle : ICssStyle{

        public void Update(HTMLDocument document){

            var styleSheet = document.createStyleSheet("", 0);

            styleSheet.cssText = @"
                .highlight{
                    color: #F00;
                    font-weight: bold;
                    text-decoration: underline;
                }
            ";
        }
    }
}
