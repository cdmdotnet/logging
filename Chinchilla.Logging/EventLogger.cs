#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="Chinchilla Software Limited">
// // 	Copyright Chinchilla Software Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chinchilla.Logging.Configuration;
using Newtonsoft.Json;

namespace Chinchilla.Logging
{
	/// <summary>
	/// Provides a set of methods that help you log events relating to the execution of your code outputting to the Windows EventLog
	/// </summary>
	public class EventLogger : PrimitiveLogger
	{
		static object OutputLock { get; set; }

		static EventLogger()
		{
			OutputLock = new object();
		}

		/// <summary>
		/// Instantiates a new instance of the <see cref="EventLogger"/> class.
		/// </summary>
		public EventLogger(ILoggerSettings loggerSettings, ICorrelationIdHelper correlationIdHelper, ITelemetryHelper telemetryHelper = null)
			: base(loggerSettings, correlationIdHelper, telemetryHelper)
		{
			if (!EventLog.SourceExists(loggerSettings.ModuleName))
				EventLog.CreateEventSource(loggerSettings.ModuleName, "Application");
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
				Log(EventLogEntryType.Information, "Sensitive", message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// This is for logging general information, effectively the least amount of information you'd want to know about a system operation,
		/// to the <see cref="Console"/> in <see cref="ConsoleColor.Gray"/>
		/// Don't abuse this as you will flood the logs as this would normally never turned off. Use <see cref="LogDebug"/> or <see cref="LogProgress"/> for reporting additional information.
		/// </summary>
		public override void LogInfo(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableInfo)
				Log(EventLogEntryType.Information, "Info", message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// Writes logging progress information such as "Process X is 24% done"
		/// to the <see cref="Console"/> in <see cref="ConsoleColor.Gray"/>
		/// </summary>
		public override void LogProgress(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableProgress)
				Log(EventLogEntryType.Information, "Progress", message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// Writes diagnostic information 
		/// to the <see cref="Console"/> in <see cref="ConsoleColor.Blue"/>
		/// </summary>
		public override void LogDebug(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableDebug)
				Log(EventLogEntryType.Information, "Debug", message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// Writes warnings, something not yet an error, but something to watch out for,
		/// to the <see cref="Console"/> in <see cref="ConsoleColor.DarkYellow"/>
		/// </summary>
		public override void LogWarning(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableWarning)
				Log(EventLogEntryType.Warning, "Warning", message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// Writes errors, something handled and to be investigated,
		/// to the <see cref="Console"/> in <see cref="ConsoleColor.DarkRed"/>
		/// </summary>
		public override void LogError(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableError)
				Log(EventLogEntryType.Error, "Error", message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// Writes fatal errors that have a detrimental effect on the system,
		/// to the <see cref="Console"/> in <see cref="ConsoleColor.Red"/>
		/// </summary>
		public override void LogFatalError(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableFatalError)
				Log(EventLogEntryType.Error, "Fatal", message, container, exception, additionalData, metaData);
		}

		#endregion

		/// <summary>
		/// Format a message based on the input parameters to be sent to the <see cref="Console"/>
		/// </summary>
		protected virtual void Log(EventLogEntryType eventLogEntryType, string level, string message, string container, Exception exception, IDictionary<string, object> additionalData, IDictionary<string, object> metaData)
		{
			string messageToLog = GenerateLogMessage(message, level, container, exception, additionalData, metaData);
			Action logAction = () =>
			{
				lock (OutputLock)
				{
					EventLog.WriteEntry(LoggerSettings.ModuleName, messageToLog, eventLogEntryType);
				}
			};

			Log(logAction, level, container);
		}
		/// <summary>
		/// Format a message based on the input parameters.
		/// </summary>
		protected virtual string GenerateLogMessage(string level, string message, string container, Exception exception, IDictionary<string, object> additionalData, IDictionary<string, object> metaData)
		{
			container = UseOrBuildContainerName(container);

			Guid corrolationId = Guid.Empty;
			try
			{
				corrolationId = CorrelationIdHelper.GetCorrelationId();
			}
			catch
			{
				// Default already set
			}

			string pattern = "Level: {0}\r\nTime: {1:r}\r\n";
			if (corrolationId != Guid.Empty)
				pattern = "Correlation ID: {7:N}\r\nTime: {1:r}\r\n";
			if (!string.IsNullOrWhiteSpace(container))
				pattern = string.Concat(pattern, "Container: {3}\r\n");
			pattern = string.Concat(pattern, "Message: {2}\r\n");
			if (exception != null)
				pattern = string.Concat(pattern, "Exception:\r\n{4}\r\n");
			if (additionalData != null)
				pattern = string.Concat(pattern, "Additional Data:\r\n{8}\r\n");
			if (metaData != null)
				pattern = string.Concat(pattern, "Meta Data:\r\n{9}\r\n");
			string messageToLog = string.Format(pattern, level, // 0
				DateTime.Now, // 1
				message, // 2
				container, // 3
				exception, // 4
				exception == null ? null : exception.Message, // 5
				exception == null ? null : exception.StackTrace, // 6
				corrolationId, // 7
				additionalData == null || !additionalData.Any() ? null : JsonConvert.SerializeObject(additionalData), // 8
				metaData == null || !metaData.Any() ? null : JsonConvert.SerializeObject(metaData) // 9
			);

			return messageToLog;
		}
	}
}