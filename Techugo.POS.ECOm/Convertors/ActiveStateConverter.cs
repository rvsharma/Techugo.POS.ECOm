using System;
using System.Globalization;
using System.Windows.Data;

namespace Techugo.POS.ECOm.Converters
{
    public class ActiveStateConverter : IValueConverter
    {
        // Converts source (string "Active"/"Inactive" or bool) -> bool for ToggleButton.IsChecked
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b) return b;
            if (value is string s)
            {
                return string.Equals(s, "Active", StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(s, "true", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        // Converts back from ToggleButton (bool) -> targetType expected by source (string or bool)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isChecked = value is bool b && b;
            if (targetType == typeof(string))
                return isChecked ? "Active" : "Inactive";
            if (targetType == typeof(bool) || targetType == typeof(bool?))
                return isChecked;
            // fallback
            return isChecked ? "Active" : "Inactive";
        }
    }
}