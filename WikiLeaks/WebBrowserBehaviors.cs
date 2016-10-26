using System;
using System.Windows;
using System.Windows.Controls;

namespace WikiLeaks{

    public static class WebBrowserBehaviors{

        public static readonly DependencyProperty HtmlProperty 
            = DependencyProperty.RegisterAttached("Html", typeof(string), typeof(WebBrowserBehaviors), new FrameworkPropertyMetadata(OnHtmlChanged));

        [AttachedPropertyBrowsableForType(typeof(WebBrowser))]
        public static string GetHtml(WebBrowser d) {
            return (string)d.GetValue(HtmlProperty);
        }

        public static void SetHtml(WebBrowser d, string value) {
            d.SetValue(HtmlProperty, value);
        }

        static void OnHtmlChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e) {
            WebBrowser webBrowser = dependencyObject as WebBrowser;
            if (webBrowser != null)
                webBrowser.NavigateToString(e.NewValue as string ?? "&nbsp;");
        }

        public static readonly DependencyProperty BindableSourceProperty =
            DependencyProperty.RegisterAttached("BindableSource", typeof(object), typeof(WebBrowserBehaviors),
                new UIPropertyMetadata(null, BindableSourcePropertyChanged));

        public static object GetBindableSource(DependencyObject obj){
            return (string) obj.GetValue(BindableSourceProperty);
        }

        public static void SetBindableSource(DependencyObject obj, object value){
            obj.SetValue(BindableSourceProperty, value);
        }

        public static void BindableSourcePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e){
            var browser = o as WebBrowser;

            if (browser == null)
                return;

            var uriString = e.NewValue as string;

            if (uriString != null){
                browser.Source = string.IsNullOrWhiteSpace(uriString) ? null : new Uri(uriString); 
                return;
            }

            browser.Source = e.NewValue as Uri;
        }
    }
}
