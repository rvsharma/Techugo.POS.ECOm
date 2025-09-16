using System.Windows;
using System.Windows.Controls;

namespace Techugo.POS.ECOm.Pages
{
    public partial class TotalOrdersPage : UserControl
    {
        public event RoutedEventHandler BackRequested;

        public TotalOrdersPage()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BackRequested?.Invoke(this, new RoutedEventArgs());
        }
    }
}