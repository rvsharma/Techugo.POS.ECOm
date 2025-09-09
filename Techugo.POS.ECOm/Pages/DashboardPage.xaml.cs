using System.Windows;
using System.Windows.Controls;

namespace Techugo.POS.ECOm.Pages
{
    public partial class DashboardPage : UserControl
    {
        public event RoutedEventHandler TotalOrdersClicked;
        public event RoutedEventHandler PickListClicked;
        // Add events for other tiles

        public DashboardPage()
        {
            InitializeComponent();
        }

        private void TotalOrders_Click(object sender, RoutedEventArgs e)
        {
            TotalOrdersClicked?.Invoke(this, new RoutedEventArgs());
        }

        private void PickList_Click(object sender, RoutedEventArgs e)
        {
            PickListClicked?.Invoke(this, new RoutedEventArgs());
        }
        // Add handlers for other tiles
    }
}