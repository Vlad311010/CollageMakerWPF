using System.Globalization;
using System.Windows.Data;

namespace WpfTestApp.Utils.Converters
{
    public class FloatToPercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float floatValue)
                return ((int)(floatValue * 100)) + "%";

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
