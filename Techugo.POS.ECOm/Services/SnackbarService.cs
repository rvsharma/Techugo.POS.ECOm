using MaterialDesignThemes.Wpf;

namespace Techugo.POS.ECOm.Services
{
    public static class SnackbarService
    {
        // Shared global message queue
        public static SnackbarMessageQueue Instance { get; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));

        public static void Enqueue(string message) => Instance.Enqueue(message);

        // optional overloads
        public static void Enqueue(object message) => Instance.Enqueue(message);
    }
}
