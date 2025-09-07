using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using Unutmaz.Models;

namespace Unutmaz.Converters.AlisverisConverters
{
    public class AllCheckedToStrikethroughConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<AlisverisOgesiModel> listeOgeleri)
            {
                // Eğer liste boş değilse ve tüm öğeler seçiliyse strikethrough uygula
                if (listeOgeleri.Count > 0 && listeOgeleri.All(x => x.IsChecked))
                {
                    return TextDecorations.Strikethrough;
                }
            }
            return TextDecorations.None;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}