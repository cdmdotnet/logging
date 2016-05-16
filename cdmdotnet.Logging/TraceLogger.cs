#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="cdmdotnet Limited">
// // 	Copyright cdmdotnet Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System;
using System.Diagnostics;
using cdmdotnet.Logging.Configuration;

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
		/// Writes an informational message to <see cref="LogInfo(string)"/>
		/// </summary>
		public void LogInfo(string message, string container = null, Exception exception = null)
		{
			if (LoggerSettings.EnableInfo)
				Log("Info", LogInfo, message, container, exception);
		}

		/// <summary>
		/// Writes a debugging message to <see cref="LogDebug(string)"/>
		/// </summary>
		public void LogDebug(string message, string container = null, Exception exception = null)
		{
			if (LoggerSettings.EnableDebug)
				Log("Debug", LogDebug, message, container, exception);
		}

		/// <summary>
		/// Writes a warning message to <see cref="LogWarning(string)"/>
		/// </summary>
		public void LogWarning(string message, string container = null, Exception exception = null)
		{
			if (LoggerSettings.EnableWarning)
				Log("Warning", LogWarning, message, container, exception);
		}

		/// <summary>
		/// Writes an error message to <see cref="LogError(string)"/>
		/// </summary>
		public void LogError(string message, string container = null, Exception exception = null)
		{
			if (LoggerSettings.EnableError)
				Log("Error", LogError, message, container, exception);
		}

		/// <summary>
		/// Writes a fatal error message to <see cref="LogFatalError(string)"/>
		/// </summary>
		public void LogFatalError(string message, string container = null, Exception exception = null)
		{
			if (LoggerSettings.EnableFatalError)
				Log("Fatal", LogFatalError, message, container, exception);
		}

		#endregion

		/// <summary>
		/// Format a message based on the input parameters to be sent to <paramref name="logAction"></paramref>
		/// </summary>
		protected virtual void Log(string level, Action<string> logAction, string message, string container, Exception exception)
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
			string messageToLog = string.Format(pattern, level, // 0
				DateTime.Now, // 1
				message, // 2
				container, // 3
				exception, // 4
				exception == null ? null : exception.Message, // 5
				exception == null ? null : exception.StackTrace, // 6
				corrolationId // 7
			);

			logAction(messageToLog);
		}

		/// <summary>
		/// Writes an informational message to <see cref="Trace.TraceInformation(string)"/>
		/// </summary>
		public void LogInfo(string message)
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