using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WikiLeaks{

    public sealed class BoolToColorConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var validated = value as bool?;

            if (validated == true){
                return new SolidColorBrush(Color.FromRgb(152, 251, 152));
            }
            
            if(validated == false) { 
                return new SolidColorBrush(Color.FromRgb(240, 128, 128));
            }

            return new SolidColorBrush(Color.FromRgb(192, 192, 204));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {

            return null;
        }
    }
}


