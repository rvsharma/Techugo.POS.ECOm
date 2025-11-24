
using System.Windows;
using System.Windows.Input;

namespace Techugo.POS.ECOm
{
    public static class MouseWheelForwarder
    {
        public static readonly DependencyProperty ForwardToParentProperty =
            DependencyProperty.RegisterAttached(
                "ForwardToParent",
                typeof(bool),
                typeof(MouseWheelForwarder),
                new PropertyMetadata(false, OnForwardToParentChanged));

        public static bool GetForwardToParent(DependencyObject obj) =>
            (bool)obj.GetValue(ForwardToParentProperty);

        public static void SetForwardToParent(DependencyObject obj, bool value) =>
            obj.SetValue(ForwardToParentProperty, value);

        private static void OnForwardToParentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                if ((bool)e.NewValue)
                    element.PreviewMouseWheel += ForwardMouseWheel;
                else
                    element.PreviewMouseWheel -= ForwardMouseWheel;
            }
        }

        private static void ForwardMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                {
                    RoutedEvent = UIElement.MouseWheelEvent,
                    Source = sender
                };

                var parent = ((FrameworkElement)sender).Parent as UIElement;
                parent?.RaiseEvent(eventArg);
            }
        }
    }
}
