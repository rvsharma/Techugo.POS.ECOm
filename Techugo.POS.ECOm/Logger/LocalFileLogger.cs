using System;
using System.Collections;
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
                        sw.WriteLine(FormatException(ex));
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

        private static string FormatException(Exception ex)
        {
            var sb = new StringBuilder();
            FormatExceptionInternal(ex, sb, 0);
            return sb.ToString();
        }

        private static void FormatExceptionInternal(Exception ex, StringBuilder sb, int level)
        {
            if (ex == null) return;

            var indent = new string(' ', level * 2);
            sb.AppendLine($"{indent}Exception Type : {ex.GetType().FullName}");
            sb.AppendLine($"{indent}Message        : {ex.Message}");
            sb.AppendLine($"{indent}StackTrace     :");
            if (!string.IsNullOrEmpty(ex.StackTrace))
            {
                foreach (var line in ex.StackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
                    sb.AppendLine($"{indent}  {line}");
            }
            else
            {
                sb.AppendLine($"{indent}  (no stack trace)");
            }

            // Exception.Data
            if (ex.Data != null && ex.Data.Count > 0)
            {
                sb.AppendLine($"{indent}Data:");
                foreach (DictionaryEntry entry in ex.Data)
                {
                    sb.AppendLine($"{indent}  {entry.Key} = {entry.Value}");
                }
            }

            // AggregateException inner exceptions
            if (ex is AggregateException aex)
            {
                var i = 0;
                foreach (var inner in aex.InnerExceptions)
                {
                    sb.AppendLine($"{indent}Aggregate InnerException[{i}]:");
                    FormatExceptionInternal(inner, sb, level + 1);
                    i++;
                }
            }
            else if (ex.InnerException != null)
            {
                sb.AppendLine($"{indent}InnerException:");
                FormatExceptionInternal(ex.InnerException, sb, level + 1);
            }
        }
    }
}
