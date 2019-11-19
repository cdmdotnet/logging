#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="Chinchilla Software Limited">
// // 	Copyright Chinchilla Software Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using Chinchilla.Logging.Configuration;

namespace Chinchilla.Logging
{
	/// <summary>
	/// Provides a set of methods that help you log events relating to the execution of your code when using persistence that is single string based, like a file, event log or tracer.
	/// </summary>
	public abstract class StringLogger : PrimitiveLogger
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="StringLogger"/> class.
		/// </summary>
		protected StringLogger(ILoggerSettings loggerSettings, ICorrelationIdHelper correlationIdHelper, ITelemetryHelper telemetryHelper)
			: base(loggerSettings, correlationIdHelper, telemetryHelper)
		{
		}

		#region Implementation of ILogger

		/// <summary>
		/// This is for logging sensitive information,
		/// to <see cref="LogInfoString(string)"/>
		/// Depending on the implementation this won't be obscured or encrypted in anyway. Use this sparingly.
		/// </summary>
		public override void LogSensitive(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableSensitive)
			{
				Log("Sensitive", LogSensitiveString, message, container, exception, additionalData, metaData);
				TelemetryHelper.TrackEvent("LogSensitive/Enabled Call");
			}
			else
				TelemetryHelper.TrackEvent("LogSensitive/Disabled Call");
		}

		/// <summary>
		/// This is for logging general information, effectively the least amount of information you'd want to know about a system operation,
		/// to <see cref="LogInfoString(string)"/>
		/// Don't abuse this as you will flood the logs as this would normally never turned off. Use <see cref="LogDebug(string,string,System.Exception,System.Collections.Generic.IDictionary{string,object},System.Collections.Generic.IDictionary{string,object})"/> or <see cref="LogProgress(string,string,System.Exception,System.Collections.Generic.IDictionary{string,object},System.Collections.Generic.IDictionary{string,object})"/> for reporting additional information.
		/// </summary>
		public override void LogInfo(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableInfo)
			{
				Log("Info", LogInfoString, message, container, exception, additionalData, metaData);
				TelemetryHelper.TrackEvent("LogInfo/Enabled Call");
			}
			else
				TelemetryHelper.TrackEvent("LogInfo/Disabled Call");
		}

		/// <summary>
		/// Writes logging progress information such as "Process X is 24% done"
		/// to <see cref="LogInfoString(string)"/>
		/// </summary>
		public override void LogProgress(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableProgress)
			{
				Log("Progress", LogProgressString, message, container, exception, additionalData, metaData);
				TelemetryHelper.TrackEvent("LogProgress/Enabled Call");
			}
			else
				TelemetryHelper.TrackEvent("LogProgress/Disabled Call");
		}

		/// <summary>
		/// Writes diagnostic information 
		/// to <see cref="LogDebugString(string)"/>
		/// </summary>
		public override void LogDebug(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableDebug)
			{
				Log("Debug", LogDebugString, message, container, exception, additionalData, metaData);
				TelemetryHelper.TrackEvent("LogDebug/Enabled Call");
			}
			else
				TelemetryHelper.TrackEvent("LogDebug/Disabled Call");
		}

		/// <summary>
		/// Writes warnings, something not yet an error, but something to watch out for,
		/// to <see cref="LogWarningString(string)"/>
		/// </summary>
		public override void LogWarning(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableWarning)
			{
				Log("Warning", LogWarningString, message, container, exception, additionalData, metaData);
				TelemetryHelper.TrackEvent("LogWarning/Enabled Call");
			}
			else
				TelemetryHelper.TrackEvent("LogWarning/Disabled Call");
		}

		/// <summary>
		/// Writes errors, something handled and to be investigated,
		/// to <see cref="LogErrorString(string)"/>
		/// </summary>
		public override void LogError(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableError)
			{
				Log("Error", LogErrorString, message, container, exception, additionalData, metaData);
				TelemetryHelper.TrackEvent("LogError/Enabled Call");
			}
			else
				TelemetryHelper.TrackEvent("LogError/Disabled Call");
		}

		/// <summary>
		/// Writes fatal errors that have a detrimental effect on the system,
		/// to <see cref="LogFatalErrorString(string)"/>
		/// </summary>
		public override void LogFatalError(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableFatalError)
			{
				Log("Fatal", LogFatalErrorString, message, container, exception, additionalData, metaData);
				TelemetryHelper.TrackEvent("LogFatalError/Enabled Call");
			}
			else
				TelemetryHelper.TrackEvent("LogFatalError/Disabled Call");
		}

		#endregion

		/// <summary>
		/// Format a message based on the input parameters to be sent to <paramref name="logAction"></paramref>
		/// </summary>
		protected virtual void Log(string level, Action<string> logAction, string message, string container, Exception exception, IDictionary<string, object> additionalData, IDictionary<string, object> metaData)
		{
			string messageToLog = GenerateLogMessage(level, message, container, exception, additionalData, metaData);

			Log(() => logAction(messageToLog), level, container);
		}

		/// <summary>
		/// Writes sensitive information to the string based logger.
		/// </summary>
		protected abstract void LogSensitiveString(string message);

		/// <summary>
		/// Writes an informational message to the string based logger.
		/// </summary>
		protected abstract void LogInfoString(string message);

		/// <summary>
		/// Writes logging progress information such as "Process X is 24% done" to the string based logger.
		/// </summary>
		protected abstract void LogProgressString(string message);

		/// <summary>
		/// Writes a debugging message to the string based logger.
		/// </summary>
		protected abstract void LogDebugString(string message);

		/// <summary>
		/// Writes a warning message to the string based logger.
		/// </summary>
		protected abstract void LogWarningString(string message);

		/// <summary>
		/// Writes an error message to the string based logger.
		/// </summary>
		protected abstract void LogErrorString(string message);

		/// <summary>
		/// Writes a fatal error message to the string based logger.
		/// </summary>
		protected abstract void LogFatalErrorString(string message);
	}
}