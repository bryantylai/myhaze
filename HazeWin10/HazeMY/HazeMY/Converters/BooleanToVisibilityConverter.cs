using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace HazeMY.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool)
            {
                if (parameter != null && string.Compare(parameter.ToString(), "Visible", StringComparison.CurrentCultureIgnoreCase) != 0)
                {
                    return (bool)value ? Visibility.Collapsed : Visibility.Visible;
                }

                return (bool)value ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
