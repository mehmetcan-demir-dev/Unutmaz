using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.Diagnostics;

namespace Unutmaz.Converters.AlisverisConverters
{
    public class ModToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Debug için log ekleyelim
            Debug.WriteLine($"ModToColorConverter - Gelen değer: '{value}', Tip: {value?.GetType().Name}");

            if (value is string mod)
            {
                mod = mod.Trim();
                Debug.WriteLine($"ModToColorConverter - Temizlenmiş mod: '{mod}'");

                if (mod.Equals("Serbest", StringComparison.OrdinalIgnoreCase))
                {
                    Debug.WriteLine("ModToColorConverter - Kırmızı renk döndürülüyor");
                    return Colors.Red; // Basit kırmızı renk
                }
                if (mod.Equals("Standart", StringComparison.OrdinalIgnoreCase))
                {
                    Debug.WriteLine("ModToColorConverter - Mavi renk döndürülüyor");
                    return Colors.SteelBlue; // Basit mavi renk
                }
            }

            Debug.WriteLine("ModToColorConverter - Varsayılan gri renk döndürülüyor");
            return Colors.Gray; // Hatalı ya da boş değerlerde varsayılan
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}