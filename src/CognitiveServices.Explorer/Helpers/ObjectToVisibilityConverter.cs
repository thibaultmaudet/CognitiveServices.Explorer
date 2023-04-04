using System.Collections;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace CognitiveServices.Explorer.Helpers;

public class ObjectToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (parameter == null)
            return value != null ? Visibility.Collapsed : Visibility.Visible;
            
        if (value != null && parameter as string == "count:1")
        {
            var property = typeof(ICollection).GetProperty("Count");

            return (int?)property?.GetValue(value, null) != 1 ? Visibility.Collapsed : Visibility.Visible;
        }

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
