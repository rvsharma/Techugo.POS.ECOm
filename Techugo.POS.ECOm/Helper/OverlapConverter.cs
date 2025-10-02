using System;
using System.Globalization;
using System.Windows.Data;

namespace Techugo.POS.ECOm.Helper
{
    public class OverlapConverter : IValueConverter
    {
        // Converts the item index to a horizontal offset for overlapping images
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int index)
            {
                // Overlap half of the image width (18px for 36px image)
                return index * 18;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
