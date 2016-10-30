using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using WikiLeaks.Enums;

namespace WikiLeaks.Converters{

    public sealed class SignatureValidationToColorConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var validated = (SignatureValidation) value;

            switch (validated){
                case SignatureValidation.NoPublicKey:
                    return new SolidColorBrush(Color.FromRgb(254, 204, 92));
                case SignatureValidation.Valid:
                    return new SolidColorBrush(Color.FromRgb(152, 251, 152));
                case SignatureValidation.Invalid:
                    return new SolidColorBrush(Color.FromRgb(240, 128, 128));
                default:
                    return new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;
        }
    }
}


