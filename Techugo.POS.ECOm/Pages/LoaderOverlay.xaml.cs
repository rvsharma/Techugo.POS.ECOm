using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Techugo.POS.ECOm.Pages
{
    public partial class LoaderOverlay : UserControl
    {
        public LoaderOverlay()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Start the storyboard from resources (avoids using UserControl.Triggers which sometimes confuses the designer)
            if (Resources["DotsStoryboard"] is Storyboard sb)
            {
                sb.Begin(this, true);
            }
        }
    }
}