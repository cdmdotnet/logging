#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="cdmdotnet Limited">
// // 	Copyright cdmdotnet Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using cdmdotnet.Logging.Configuration;

namespace cdmdotnet.Logging
{
	/// <summary>
	/// Provides a set of methods that help you log events relating to the execution of your code outputting to the <see cref="Console"/>
	/// </summary>
	public class ConsoleLogger : PrimitiveLogger
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="ConsoleLogger"/> class.
		/// </summary>
		public ConsoleLogger(ILoggerSettings loggerSettings, ICorrelationIdHelper correlationIdHelper, ITelemetryHelper telemetryHelper = null)
			: base(loggerSettings, correlationIdHelper, telemetryHelper)
		{
		}

		#region Implementation of ILogger

		/// <summary>
		/// This is for logging sensitive information,
		/// to the <see cref="Console"/> in <see cref="ConsoleColor.Gray"/>
		/// Depending on the implementation this won't be obscured or encrypted in anyway. Use this sparingly.
		/// </summary>
		public override void LogSensitive(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableSensitive)
				Log("Sensitive", ConsoleColor.DarkYellow, message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// This is for logging general information, effectively the least amount of information you'd want to know about a system operation,
		/// to the <see cref="Console"/> in <see cref="ConsoleColor.Gray"/>
		/// Don't abuse this as you will flood the logs as this would normally never turned off. Use <see cref="LogDebug"/> or <see cref="LogProgress"/> for reporting additional information.
		/// </summary>
		public override void LogInfo(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableInfo)
				Log("Info", ConsoleColor.Gray, message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// Writes logging progress information such as "Process X is 24% done"
		/// to the <see cref="Console"/> in <see cref="ConsoleColor.Gray"/>
		/// </summary>
		public override void LogProgress(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableProgress)
				Log("Progress", ConsoleColor.Gray, message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// Writes diagnostic information 
		/// to the <see cref="Console"/> in <see cref="ConsoleColor.Blue"/>
		/// </summary>
		public override void LogDebug(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableDebug)
				Log("Debug", ConsoleColor.Blue, message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// Writes warnings, something not yet an error, but something to watch out for,
		/// to the <see cref="Console"/> in <see cref="ConsoleColor.DarkYellow"/>
		/// </summary>
		public override void LogWarning(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableWarning)
				Log("Warning", ConsoleColor.DarkYellow, message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// Writes errors, something handled and to be investigated,
		/// to the <see cref="Console"/> in <see cref="ConsoleColor.DarkRed"/>
		/// </summary>
		public override void LogError(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableError)
				Log("Error", ConsoleColor.DarkRed, message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// Writes fatal errors that have a detrimental effect on the system,
		/// to the <see cref="Console"/> in <see cref="ConsoleColor.Red"/>
		/// </summary>
		public override void LogFatalError(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableFatalError)
				Log("Fatal", ConsoleColor.Red, message, container, exception, additionalData, metaData);
		}

		#endregion

		/// <summary>
		/// Format a message based on the input parameters to be sent to the <see cref="Console"/>
		/// </summary>
		protected virtual void Log(string level, ConsoleColor foregroundColor, string message, string container, Exception exception, IDictionary<string, object> additionalData, IDictionary<string, object> metaData)
		{
			string messageToLog = GenerateLogMessage(level, message, container, exception, additionalData, metaData);

			Action logAction = () =>
			{
				ConsoleColor originalColour = Console.ForegroundColor;
				Console.ForegroundColor = foregroundColor;
				Console.WriteLine(messageToLog);
				Console.ForegroundColor = originalColour;
			};

			Log(logAction, level, container);
		}
	}
}