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
	public class TraceLogger : ILogger
	{
		public TraceLogger(ILoggerSettings loggerSettings, ICorrelationIdHelper correlationIdHelper)
		{
			LoggerSettings = loggerSettings;
			CorrelationIdHelper = correlationIdHelper;
		}

		protected ILoggerSettings LoggerSettings { get; private set; }

		protected ICorrelationIdHelper CorrelationIdHelper { get; private set; }

		#region Implementation of ILog

		public void LogInfo(string message, string container = null, Exception exception = null)
		{
			if (LoggerSettings.EnableInfo)
				Log("Info", LogInfo, message, container, exception);
		}

		public void LogDebug(string message, string container = null, Exception exception = null)
		{
			if (LoggerSettings.EnableDebug)
				Log("Debug", LogDebug, message, container, exception);
		}

		public void LogWarning(string message, string container = null, Exception exception = null)
		{
			if (LoggerSettings.EnableWarning)
				Log("Warning", LogWarning, message, container, exception);
		}

		public void LogError(string message, string container = null, Exception exception = null)
		{
			if (LoggerSettings.EnableError)
				Log("Error", LogError, message, container, exception);
		}

		public void LogFatalError(string message, string container = null, Exception exception = null)
		{
			if (LoggerSettings.EnableFatalError)
				Log("Fatal", LogFatalError, message, container, exception);
		}

		#endregion

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

		public void LogInfo(string message)
		{
			Trace.TraceInformation(message);
		}

		public void LogDebug(string message)
		{
			Trace.TraceInformation(message);
		}

		public void LogWarning(string message)
		{
			Trace.TraceWarning(message);
		}

		public void LogError(string message)
		{
			Trace.TraceError(message);
		}

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