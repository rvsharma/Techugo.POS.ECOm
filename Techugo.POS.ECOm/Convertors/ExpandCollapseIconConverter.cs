using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Techugo.POS.ECOm.Convertors
{
    public class ExpandCollapseIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isExpanded = value is bool b && b;
            // ▼ (down arrow) for expanded, ▶ (right arrow) for collapsed
            return isExpanded
                ? "/Assets/Images/collapse.svg"
                : "/Assets/Images/expand.svg";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
