using System.Windows;
using System.Windows.Controls;

namespace Techugo.POS.ECOm.Pages
{
    public partial class SalesDashboardPage : UserControl
    {
        public event RoutedEventHandler BackRequested;

        public SalesDashboardPage()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BackRequested?.Invoke(this, new RoutedEventArgs());
        }
    }
}