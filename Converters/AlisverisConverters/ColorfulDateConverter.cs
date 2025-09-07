using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Unutmaz.Converters.AlisverisConverters
{
    public class ColorfulDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime tarih)
            {
                double fark = (tarih.Date - DateTime.Now.Date).TotalDays;

                if (fark > 3)
                    return Colors.Green;   // Güvenli
                else if (fark >= 1)
                    return Colors.Orange;  // Uyarı
                else if (fark == 0)
                    return Colors.Red;     // Acil (Bugün)
                else
                    return Colors.Gray;    // Geçmiş tarih
            }

            return Colors.Gray; // Değer DateTime değilse veya null ise
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
