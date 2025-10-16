using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Techugo.POS.ECOm.Converters
{
    /// <summary>
    /// Converts an order status string to a background brush.
    /// Case-insensitive, returns a sensible default when unknown.
    /// </summary>
    public class StatusToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (value ?? string.Empty).ToString().Trim().ToLowerInvariant();

            return status switch
            {
                "delivered" => (SolidColorBrush)(new BrushConverter().ConvertFrom("#dcfce7")),   // pale green
                "placed" => (SolidColorBrush)(new BrushConverter().ConvertFrom("#E6FFFA")),       // teal-ish
                "pending" => (SolidColorBrush)(new BrushConverter().ConvertFrom("#FEF3C7")),     // pale yellow
                "processing" => (SolidColorBrush)(new BrushConverter().ConvertFrom("#F3F4F6")),  // light gray
                "accepted" or "in-transit" or "out for delivery" => (SolidColorBrush)(new BrushConverter().ConvertFrom("#dbeafe")), // pale blue
                "rejected" or "cancelled" or "canceled" => (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffe2e2")), // pale red
                _ => (SolidColorBrush)(new BrushConverter().ConvertFrom("#F3F4F6")), // default light gray
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }
}