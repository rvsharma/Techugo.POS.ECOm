using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Techugo.POS.ECOm.Pages
{
    public partial class NewOrderPopUp : UserControl
    {
        public event RoutedEventHandler AcceptOrderClicked;
        public event RoutedEventHandler RejectOrderClicked;

        public NewOrderPopUp()
        {
            InitializeComponent();
        }

        private void AcceptOrder_Click(object sender, RoutedEventArgs e)
        {
            AcceptOrderClicked?.Invoke(this, new RoutedEventArgs());
        }

        private void RejectOrder_Click(object sender, RoutedEventArgs e)
        {
            RejectOrderClicked?.Invoke(this, new RoutedEventArgs());
        }
        private void PopupBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Only allow drag if hosted in a Window
            if (e.ChangedButton == MouseButton.Left)
            {
                Window parentWindow = Window.GetWindow(this);
                parentWindow?.DragMove();
            }
        }
    }
}
