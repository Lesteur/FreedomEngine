using System.Diagnostics;

namespace FreedomEngine.Core
{
    public static class Logger
    {
        public static void Info(string message)
        {
            Write("INFO", message);
        }

        public static void Warning(string message)
        {
            Write("WARN", message);
        }

        public static void Error(string message)
        {
            Write("ERROR", message);
        }

        private static void Write(string level, string message)
        {
            Debug.WriteLine($"[{level}] {message}");
        }
    }
}