using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Techugo.POS.ECOm.Pages.Dashboard
{
    /// <summary>
    /// Interaction logic for PartialReturns.xaml
    /// </summary>
    public partial class PartialReturns : UserControl
    {
        public event RoutedEventHandler BackRequested;

        public PartialReturns()
        {
            InitializeComponent();
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BackRequested?.Invoke(this, new RoutedEventArgs());
        }
    }
}
