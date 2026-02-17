using System.Windows;
using System.Windows.Controls;

namespace Techugo.POS.ECOm.Pages
{
    public partial class RefundManagementPage : UserControl
    {
        public event RoutedEventHandler BackRequested;

        public RefundManagementPage()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BackRequested?.Invoke(this, new RoutedEventArgs());
        }
    }
}