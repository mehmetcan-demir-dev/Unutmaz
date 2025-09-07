using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Unutmaz.Converters.AlisverisConverters
{
    public class ModToShortLetterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string mod = value as string;
            if (string.IsNullOrEmpty(mod))
                return "?";

            return mod.ToLower() switch
            {
                "serbest" => "S",
                "standart" => "M",
                _ => "?"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
