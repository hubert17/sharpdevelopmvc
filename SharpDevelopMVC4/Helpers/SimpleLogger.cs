using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Hosting;


public static class SimpleLogger
{
    public static string DatetimeFormat;
    public static string Filename;

    /// <summary>
    /// Initialize a new instance of SimpleLogger class.
    /// Log file will be created automatically if not yet exists, else it can be either a fresh new file or append to the existing file.
    /// Default is create a fresh new log file.
    /// </summary>
    public static void Init()
    {
    	var ContentRootPath = HostingEnvironment.MapPath("~");

        DatetimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
        string Date = DateTime.UtcNow.ToString("yyyyMMdd");
        Filename = Path.Combine(ContentRootPath, "Logs", "log-" + Date + ".txt");

        if (!System.IO.File.Exists(Filename))
        {
            (new FileInfo(Filename)).Directory.Create();
            // Log file header line
            string logHeader = Filename + " is created." + Environment.NewLine;
            WriteLine(DateTime.Now.ToString(DatetimeFormat) + " " + logHeader, false);
        }
    }

    /// <summary>
    /// Log a debug message
    /// </summary>
    /// <param name="text">Message</param>
    public static void LogDebug(string text)
    {
        WriteFormattedLog(LogLevel.DEBUG, text);
    }

    /// <summary>
    /// Log an error message
    /// </summary>
    /// <param name="text">Message</param>
    public static void LogError(string text)
    {
        WriteFormattedLog(LogLevel.ERROR, text);
    }

    /// <summary>
    /// Log a fatal error message
    /// </summary>
    /// <param name="text">Message</param>
    public static void LogFatal(string text)
    {
        WriteFormattedLog(LogLevel.FATAL, text);
    }

    /// <summary>
    /// Log an info message
    /// </summary>
    /// <param name="text">Message</param>
    public static void LogInformation(string text)
    {
        WriteFormattedLog(LogLevel.INFO, text);
    }

    /// <summary>
    /// Log a trace message
    /// </summary>
    /// <param name="text">Message</param>
    public static void LogTrace(string text)
    {
        WriteFormattedLog(LogLevel.TRACE, text);
    }

    /// <summary>
    /// Log a waning message
    /// </summary>
    /// <param name="text">Message</param>
    public static void LogWarning(string text)
    {
        WriteFormattedLog(LogLevel.WARNING, text);
    }

    /// <summary>
    /// Format a log message based on log level
    /// </summary>
    /// <param name="level">Log level</param>
    /// <param name="text">Log message</param>
    private static void WriteFormattedLog(LogLevel level, string text)
    {
        string pretext;
        switch (level)
        {
            case LogLevel.TRACE: pretext = DateTime.UtcNow.ToString(DatetimeFormat) + " [TRACE]   "; break;
            case LogLevel.INFO: pretext = DateTime.UtcNow.ToString(DatetimeFormat) + " [INFO]    "; break;
            case LogLevel.DEBUG: pretext = DateTime.UtcNow.ToString(DatetimeFormat) + " [DEBUG]   "; break;
            case LogLevel.WARNING: pretext = DateTime.UtcNow.ToString(DatetimeFormat) + " [WARNING] "; break;
            case LogLevel.ERROR: pretext = DateTime.UtcNow.ToString(DatetimeFormat) + " [ERROR]   "; break;
            case LogLevel.FATAL: pretext = DateTime.UtcNow.ToString(DatetimeFormat) + " [FATAL]   "; break;
            default: pretext = ""; break;
        }

        WriteLine(pretext + text + Environment.NewLine);
    }

    /// <summary>
    /// Write a line of formatted log message into a log file
    /// </summary>
    /// <param name="text">Formatted log message</param>
    /// <param name="append">True to append, False to overwrite the file</param>
    /// <exception cref="System.IO.IOException"></exception>
    public static void WriteLine(string text, bool append = true)
    {
        if (!string.IsNullOrEmpty(text))
        {
            try
            {
                if (append)
                    File.AppendAllText(Filename, text);
                else
                    File.WriteAllText(Filename, text);
            }
            catch { }
        }
    }

    /// <summary>
    /// Supported log level
    /// </summary>
    [Flags]
    private enum LogLevel
    {
        TRACE,
        INFO,
        DEBUG,
        WARNING,
        ERROR,
        FATAL
    }

}

