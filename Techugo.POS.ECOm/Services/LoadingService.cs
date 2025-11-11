using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;

namespace Techugo.POS.ECOm.Services
{
    public class LoadingService : INotifyPropertyChanged
    {
        public static LoadingService Instance { get; } = new LoadingService();

        private int _counter;
        private bool _isLoading;
        private string _message;

        public bool IsLoading
        {
            get => _isLoading;
            private set
            {
                if (_isLoading == value) return;
                _isLoading = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            }
        }

        public string Message
        {
            get => _message;
            private set
            {
                if (_message == value) return;
                _message = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Message)));
            }
        }

        private LoadingService() { }

        // Increments counter and shows overlay. Thread-safe and marshals to UI thread.
        public void Show(string message = null)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                Interlocked.Increment(ref _counter);
                Message = message;
                IsLoading = true;
            });
        }

        // Decrements counter and hides overlay when zero.
        public void Hide()
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                if (_counter <= 0) { _counter = 0; IsLoading = false; Message = null; return; }
                Interlocked.Decrement(ref _counter);
                if (_counter == 0)
                {
                    IsLoading = false;
                    Message = null;
                }
            });
        }

        // Convenience: use with using(...) pattern
        public IDisposable Enter(string message = null)
        {
            Show(message);
            return new Releaser(this);
        }

        private class Releaser : IDisposable
        {
            private readonly LoadingService _owner;
            private bool _disposed;
            public Releaser(LoadingService owner) => _owner = owner;
            public void Dispose()
            {
                if (_disposed) return;
                _disposed = true;
                _owner.Hide();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}