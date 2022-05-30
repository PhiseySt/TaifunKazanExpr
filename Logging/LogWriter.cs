using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace TaifunKazanExpress.Logging
{
    public static class LogWriter
    {
        private static string? _logPath;

        public static void LogWrite(string logMessage)
        {
            _logPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            using StreamWriter sw =
                File.AppendText(
                    $"{_logPath}\\log{DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.CurrentCulture)}.txt");
            Log(logMessage, sw);
        }

        public static void Log(string logMessage, TextWriter txtWriter)
        {
            txtWriter.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}  : {logMessage}");
            txtWriter.WriteLine("-------------------------------");
        }
    }
}

