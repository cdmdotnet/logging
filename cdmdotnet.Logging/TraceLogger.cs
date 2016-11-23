﻿#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="cdmdotnet Limited">
// // 	Copyright cdmdotnet Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using cdmdotnet.Logging.Configuration;
using Newtonsoft.Json;

namespace cdmdotnet.Logging
{
	/// <summary>
	/// Provides a set of methods that help you log events relating to the execution of your code outputting to <see cref="Trace"/>
	/// </summary>
	public class TraceLogger : ILogger
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="TraceLogger"/> class.
		/// </summary>
		public TraceLogger(ILoggerSettings loggerSettings, ICorrelationIdHelper correlationIdHelper)
		{
			LoggerSettings = loggerSettings;
			CorrelationIdHelper = correlationIdHelper;
		}

		/// <summary>
		/// The <see cref="ILoggerSettings"/> for the instance, set during Instantiation
		/// </summary>
		protected ILoggerSettings LoggerSettings { get; private set; }

		/// <summary>
		/// The <see cref="ICorrelationIdHelper"/> for the instance, set during Instantiation
		/// </summary>
		protected ICorrelationIdHelper CorrelationIdHelper { get; private set; }

		#region Implementation of ILogger

		/// <summary>
		/// This is for logging sensitive information,
		/// to <see cref="LogInfo(string)"/>
		/// Depending on the implementation this won't be obscured or encrypted in anyway. Use this sparingly.
		/// </summary>
		public void LogSensitive(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableSensitive)
				Log("Sensitive", LogInfo, message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// This is for logging general information, effectively the least amount of information you'd want to know about a system operation,
		/// to <see cref="LogInfo(string)"/>
		/// Don't abuse this as you will flood the logs as this would normally never turned off. Use <see cref="LogDebug(string,string,System.Exception,System.Collections.Generic.IDictionary{string,object},System.Collections.Generic.IDictionary{string,object})"/> or <see cref="LogProgress(string,string,System.Exception,System.Collections.Generic.IDictionary{string,object},System.Collections.Generic.IDictionary{string,object})"/> for reporting additional information.
		/// </summary>
		public void LogInfo(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableInfo)
				Log("Info", LogSensitive, message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// Writes logging progress information such as "Process X is 24% done"
		/// to <see cref="LogInfo(string)"/>
		/// </summary>
		public void LogProgress(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableProgress)
				Log("Progress", LogProgress, message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// Writes diagnostic information 
		/// to <see cref="LogDebug(string)"/>
		/// </summary>
		public void LogDebug(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableDebug)
				Log("Debug", LogDebug, message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// Writes warnings, something not yet an error, but something to watch out for,
		/// to <see cref="LogWarning(string)"/>
		/// </summary>
		public void LogWarning(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableWarning)
				Log("Warning", LogWarning, message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// Writes errors, something handled and to be investigated,
		/// to <see cref="LogError(string)"/>
		/// </summary>
		public void LogError(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableError)
				Log("Error", LogError, message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// Writes fatal errors that have a detrimental effect on the system,
		/// to <see cref="LogFatalError(string)"/>
		/// </summary>
		public void LogFatalError(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableFatalError)
				Log("Fatal", LogFatalError, message, container, exception, additionalData, metaData);
		}

		#endregion

		/// <summary>
		/// Format a message based on the input parameters to be sent to <paramref name="logAction"></paramref>
		/// </summary>
		protected virtual void Log(string level, Action<string> logAction, string message, string container, Exception exception, IDictionary<string, object> additionalData, IDictionary<string, object> metaData)
		{
			Guid corrolationId = Guid.Empty;
			try
			{
				corrolationId = CorrelationIdHelper.GetCorrelationId();
			}
			catch { }

			string pattern = "[{0}] {1:r}:";
			if (corrolationId != Guid.Empty)
				pattern = "[{0}] [{7:N}] {1:r}:";
			if (!string.IsNullOrWhiteSpace(container))
				pattern = string.Concat(pattern, " {3}:: {2}");
			if (exception != null)
				pattern = string.Concat(pattern, "\r\n{4}");
			if (additionalData != null)
				pattern = string.Concat(pattern, "\r\n{8}");
			if (metaData != null)
				pattern = string.Concat(pattern, "\r\n{9}");
			string messageToLog = string.Format(pattern, level, // 0
				DateTime.Now, // 1
				message, // 2
				container, // 3
				exception, // 4
				exception == null ? null : exception.Message, // 5
				exception == null ? null : exception.StackTrace, // 6
				corrolationId, // 7
				JsonConvert.SerializeObject(additionalData), // 8
				JsonConvert.SerializeObject(metaData) // 9
			);

			logAction(messageToLog);
		}

		/// <summary>
		/// Writes sensitive information to <see cref="Trace.TraceInformation(string)"/>
		/// </summary>
		public void LogSensitive(string message)
		{
			Trace.TraceInformation(message);
		}

		/// <summary>
		/// Writes an informational message to <see cref="Trace.TraceInformation(string)"/>
		/// </summary>
		public void LogInfo(string message)
		{
			Trace.TraceInformation(message);
		}

		/// <summary>
		/// Writes logging progress information such as "Process X is 24% done" to <see cref="Trace.TraceInformation(string)"/>
		/// </summary>
		public void LogProgress(string message)
		{
			Trace.TraceInformation(message);
		}

		/// <summary>
		/// Writes a debugging message to <see cref="Trace.TraceInformation(string)"/>
		/// </summary>
		public void LogDebug(string message)
		{
			Trace.TraceInformation(message);
		}

		/// <summary>
		/// Writes a warning message to <see cref="Trace.TraceWarning(string)"/>
		/// </summary>
		public void LogWarning(string message)
		{
			Trace.TraceWarning(message);
		}

		/// <summary>
		/// Writes an error message to <see cref="Trace.TraceError(string)"/>
		/// </summary>
		public void LogError(string message)
		{
			Trace.TraceError(message);
		}

		/// <summary>
		/// Writes a fatal error message to <see cref="Trace.TraceError(string)"/>
		/// </summary>
		public void LogFatalError(string message)
		{
			Trace.TraceError(message);
		}

		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
		}

		#endregion
	}
}