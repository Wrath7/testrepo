using System;
using System.IO;
using System.Text;
using NLog;
using System.Collections.Generic;

namespace Illuminate.Tools
{
	/// <summary>
	/// Class to log text to a file
	/// </summary>
	public class Logger
	{
        /// <summary>
        /// The NLog instance used for logging.
        /// </summary>
        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();

		private static string _logNameToTrace = "";

		private static Dictionary<string, string> _lastLogEntry = new Dictionary<string, string>();

		public static string LogNameToTrace
		{
			get
			{
				return _logNameToTrace;
			}
			set
			{
				_logNameToTrace = value;
			}
		}

		/// <summary>
		/// Logger Severity Levels
		/// </summary>
		public enum Severity
		{
			/// <summary>
			/// Information you want to log but not an error (e.g. This class was instantiated properly)
			/// </summary>
			Debug = 0,

			/// <summary>
			/// Any debug information you want to insert in the log
			/// </summary>
			Information = 1,

			/// <summary>
			/// Errors you want to log but aren't show stoppers
			/// </summary>
			Error = 2,

			/// <summary>
			/// Must fix errors
			/// </summary>
			Fatal = 3
		}

		/// <summary>
		/// Writes a string of text to the log
		/// </summary>
		/// <param name="LogText">The text you want to write to the log</param>
		/// <param name="Level">The severity level</param>
		public static void WriteLine(string LogText, Severity Level, string Id)
		{
			if (!string.IsNullOrEmpty(Id))
				LogText = Id + "|" + LogText;

            switch (Level)
            {
                case Severity.Information:
                    logger.Info(LogText);
                    break;
                case Severity.Error:
                    logger.Error(LogText);
                    break;
                case Severity.Fatal:
                    logger.Fatal(LogText);
                    break;
                case Severity.Debug:
                    logger.Debug(LogText);
                    break;
            }

			if (_logNameToTrace == Id || _logNameToTrace == "all")
			{
			    Console.WriteLine(LogText);
			}

			if (Id != null)
			{
				if (_lastLogEntry.ContainsKey(Id))
				{
					_lastLogEntry[Id] = LogText;
				}
				else
				{
					_lastLogEntry.Add(Id, LogText);
				}
			}
		} // WriteLine

		/// <summary>
		/// Writes a string of text to the log
		/// </summary>
		/// <param name="LogText">The text you want to write to the log</param>
		/// <param name="Level">The severity level</param>
		public static void WriteLine(string LogText, Severity Level)
		{
			WriteLine(LogText, Level, null);
		} // WriteLine

		public static string GetLastLogEntry(string Id)
		{
			if (_lastLogEntry.ContainsKey(Id))
			{
				return _lastLogEntry[Id];
			}
			else
			{
				return "not found";
			}
		}
	}
}
