using System;

namespace Log_Handler
{
    class LogHandler
    {
        /// <summary>
        /// Creates a new log entry using the specified severity level
        /// </summary>
        /// <param name="e"></param>
        /// <param name="severityLevel"></param>
        public static async void CreateEntry(Exception e, SeverityLevel severit, string subjext)
        {
            // Needs implementing
        }

        public static async void CreateEntry(SeverityLevel severity, string subjext)
        {
            // Needs implementing
        }
    }

    public enum SeverityLevel
    {
        Fatal,
        Error,
        Warn,
        Info,
        Debug,
        Trace
    }
}
