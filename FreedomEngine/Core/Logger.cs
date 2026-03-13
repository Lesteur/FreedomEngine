using System.Diagnostics;

namespace FreedomEngine.Core
{
    /// <summary>
    /// Provides static methods for logging informational, warning, and error messages to the debug output.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Writes an informational message to the log output with an "INFO" prefix.
        /// </summary>
        /// <param name="message">The message to be logged as an informational entry. Cannot be null.</param>
        public static void Info(string message)
        {
            Write("INFO", message);
        }

        /// <summary>
        /// Logs a warning message with a "WARN" prefix.
        /// </summary>
        /// <param name="message">The message to log as a warning. This should provide context about the warning being issued.</param>
        public static void Warning(string message)
        {
            Write("WARN", message);
        }

        /// <summary>
        /// Logs an error message with the prefix "ERROR".
        /// </summary>
        /// <param name="message">The message to be logged as an error. This should provide context about the error that occurred.</param>
        public static void Error(string message)
        {
            Write("ERROR", message);
        }

        /// <summary>
        /// Writes a log message with the specified severity level to the debug output.
        /// </summary>
        /// <param name="level">The severity level of the log message, such as 'Info', 'Warning', or 'Error'.</param>
        /// <param name="message">The message to be logged, providing context or details about the event being logged.</param>
        private static void Write(string level, string message)
        {
            Debug.WriteLine($"[{level}] {message}");
        }
    }
}