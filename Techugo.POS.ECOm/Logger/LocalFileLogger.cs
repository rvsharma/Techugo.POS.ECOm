using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Techugo.POS.ECOm.Logger
{
    internal static class LocalFileLogger
    {
        private static readonly object _sync = new();
        private static readonly string _logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");

        static LocalFileLogger()
        {
            try { Directory.CreateDirectory(_logDirectory); } catch { }
        }

        private static string GetLogFilePath() => Path.Combine(_logDirectory, $"app-{DateTime.UtcNow:yyyyMMdd}.log");

        public static void Log(string level, string message, Exception? ex = null)
        {
            try
            {
                lock (_sync)
                {
                    var path = GetLogFilePath();
                    using var fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read);
                    using var sw = new StreamWriter(fs, Encoding.UTF8);
                    sw.WriteLine($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} [{level}] {message}");
                    if (ex != null)
                    {
                        sw.WriteLine(ex.ToString());
                    }
                }
            }
            catch
            {
                // Intentionally swallow - logging must not crash the app.
            }
        }

        public static void Info(string message) => Log("INF", message);
        public static void Debug(string message) => Log("DBG", message);
        public static void Warn(string message) => Log("WRN", message);
        public static void Error(string message, Exception? ex = null) => Log("ERR", message, ex);
    }
}
