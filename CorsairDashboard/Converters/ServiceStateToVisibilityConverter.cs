using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using CorsairDashboard.Common.Service;

namespace CorsairDashboard.Converters
{
    public class ServiceStateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var serviceState = (CorsairHydroServiceState)value;
            var isVisibileForServiceState = (CorsairHydroServiceState) parameter;
            return (serviceState == isVisibileForServiceState ? Visibility.Visible : Visibility.Collapsed);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
