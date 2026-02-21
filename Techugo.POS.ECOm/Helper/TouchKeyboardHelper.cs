using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Techugo.POS.ECOm.Helper
{
    public static class TouchKeyboardHelper
    {
        public static readonly DependencyProperty EnableProperty =
            DependencyProperty.RegisterAttached("Enable", typeof(bool), typeof(TouchKeyboardHelper), new PropertyMetadata(false, OnEnableChanged));

        public static void SetEnable(DependencyObject element, bool value) => element.SetValue(EnableProperty, value);
        public static bool GetEnable(DependencyObject element) => (bool)element.GetValue(EnableProperty);

        private static DispatcherTimer? _delayedHideTimer;

        private static void OnEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement ui)
            {
                if ((bool)e.NewValue)
                {
                    ui.GotKeyboardFocus += Ui_GotKeyboardFocus;
                    ui.LostKeyboardFocus += Ui_LostKeyboardFocus;
                    ui.PreviewMouseDown += Ui_PreviewMouseDown;
                }
                else
                {
                    ui.GotKeyboardFocus -= Ui_GotKeyboardFocus;
                    ui.LostKeyboardFocus -= Ui_LostKeyboardFocus;
                    ui.PreviewMouseDown -= Ui_PreviewMouseDown;
                }
            }
        }

        private static void Ui_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is UIElement ui) ui.Focus();
        }

        private static void Ui_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            try
            {
                _delayedHideTimer?.Stop();
                _delayedHideTimer = null;
            }
            catch { }

            ShowTouchKeyboard();
        }

        private static void Ui_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // Always hide the touch keyboard when a control loses keyboard focus.
            // This removes the previous behavior that kept the keyboard open when focus moved to another TextBox.
            HideTouchKeyboard();
        }

        public static void ShowTouchKeyboard()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

            string[] candidates =
            {
                Environment.ExpandEnvironmentVariables(@"%ProgramFiles%\Common Files\microsoft shared\ink\TabTip.exe"),
                Environment.ExpandEnvironmentVariables(@"%ProgramFiles(x86)%\Common Files\microsoft shared\ink\TabTip.exe"),
                Environment.ExpandEnvironmentVariables(@"%windir%\System32\osk.exe")
            };

            foreach (var path in candidates)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(path)) continue;
                    if (!System.IO.File.Exists(path)) continue;

                    var procName = System.IO.Path.GetFileNameWithoutExtension(path);
                    if (!Process.GetProcessesByName(procName).Any())
                    {
                        Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                    }

                    // started/exists — stop searching
                    return;
                }
                catch
                {
                    // swallow and try next candidate
                }
            }
        }

        public static void HideTouchKeyboard()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

            string[] procs = { "TabTip", "osk" };
            foreach (var name in procs)
            {
                try
                {
                    foreach (var p in Process.GetProcessesByName(name))
                    {
                        p.Kill();
                    }
                }
                catch
                {
                    // ignore
                }
            }
        }
    }
}
