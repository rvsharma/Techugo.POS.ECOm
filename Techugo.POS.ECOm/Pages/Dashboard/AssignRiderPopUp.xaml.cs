using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Techugo.POS.ECom.Model.ViewModel;


namespace Techugo.POS.ECOm.Pages.Dashboard
{
    /// <summary>
    /// Interaction logic for AssignRiderPopUp.xaml
    /// </summary>
    public partial class AssignRiderPopUp : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event RoutedEventHandler CloseClicked;
        public event RoutedEventHandler AssignRiderClicked;
        public AssignRiderPopUp(OrderDetailVM orderDetail)
        {
            InitializeComponent();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            CloseClicked?.Invoke(this, new RoutedEventArgs());
        }
        private void AssignRider_Click(object sender, RoutedEventArgs e)
        {
            AssignRiderClicked?.Invoke(this, new RoutedEventArgs());
        }
    }
}
