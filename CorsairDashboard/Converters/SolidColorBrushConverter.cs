using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace CorsairDashboard.Converters
{
    public class SolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var brush = (SolidColorBrush)value;
            var color = brush.Color;
            return String.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var color = (Color)System.Windows.Media.ColorConverter.ConvertFromString((string)value);
            return new SolidColorBrush(color);
        }
    }
}
