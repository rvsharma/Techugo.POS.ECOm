using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Techugo.POS.ECOm.Converters
{
    /// <summary>
    /// Converts an order status string to a foreground brush that contrasts the background.
    /// </summary>
    public class StatusToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (value ?? string.Empty).ToString().Trim().ToLowerInvariant();

            return status switch
            {
                "delivered" => (SolidColorBrush)(new BrushConverter().ConvertFrom("#016630")), // dark green
                "placed" => (SolidColorBrush)(new BrushConverter().ConvertFrom("#065F46")),     // dark teal
                "pending" => (SolidColorBrush)(new BrushConverter().ConvertFrom("#92400E")),   // dark amber/brown
                "processing" => (SolidColorBrush)(new BrushConverter().ConvertFrom("#374151")),// gray-700
                "accepted" or "in-transit" or "out for delivery" => (SolidColorBrush)(new BrushConverter().ConvertFrom("#193cb8")), // dark blue
                "rejected" or "cancelled" or "canceled" => (SolidColorBrush)(new BrushConverter().ConvertFrom("#9f0712")), // dark red
                _ => (SolidColorBrush)(new BrushConverter().ConvertFrom("#111827")), // default dark
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }
}