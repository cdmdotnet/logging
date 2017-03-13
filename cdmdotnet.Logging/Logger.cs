#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="cdmdotnet Limited">
// // 	Copyright cdmdotnet Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using cdmdotnet.Logging.Configuration;
using Newtonsoft.Json;

namespace cdmdotnet.Logging
{
	/// <summary>
	/// Provides a set of methods that help you log events relating to the execution of your code.
	/// </summary>
	public abstract class Logger : VeryPrimitiveLogger
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="Logger"/> class preparing the required thread pool polling if <see cref="ILoggerSettings.EnableThreadedLogging"/> is set to true.
		/// </summary>
		protected Logger(ILoggerSettings loggerSettings, ICorrelationIdHelper correlationIdHelper, ITelemetryHelper telemetryHelper)
			: base(loggerSettings, correlationIdHelper, telemetryHelper)
		{
		}

		#region Implementation of ILog

		/// <summary>
		/// This is for logging sensitive information,
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// Depending on the implementation this won't be obscured or encrypted in anyway. Use this sparingly.
		/// </summary>
		public override void LogSensitive(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableSensitive)
			{
				Log("Sensitive", message, container, exception, additionalData, metaData);
				TelemetryHelper.TrackEvent("LogSensitive/Enabled Call");
			}
			else
				TelemetryHelper.TrackEvent("LogSensitive/Disabled Call");
		}

		/// <summary>
		/// This is for logging general information, effectively the least amount of information you'd want to know about a system operation,
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// Don't abuse this as you will flood the logs as this would normally never turned off. Use <see cref="LogDebug"/> or <see cref="LogProgress"/> for reporting additional information.
		/// </summary>
		public override void LogInfo(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableInfo)
			{
				Log("Info", message, container, exception, additionalData, metaData);
				TelemetryHelper.TrackEvent("LogInfo/Enabled Call");
			}
			else
				TelemetryHelper.TrackEvent("LogInfo/Disabled Call");
		}

		/// <summary>
		/// Writes logging progress information such as "Process X is 24% done"
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		public override void LogProgress(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableProgress)
			{
				Log("Progress", message, container, exception, additionalData, metaData);
				TelemetryHelper.TrackEvent("LogProgress/Enabled Call");
			}
			else
				TelemetryHelper.TrackEvent("LogProgress/Disabled Call");
		}

		/// <summary>
		/// Writes diagnostic information 
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		public override void LogDebug(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableDebug)
			{
				Log("Debug", message, container, exception, additionalData, metaData);
				TelemetryHelper.TrackEvent("LogDebug/Enabled Call");
			}
			else
				TelemetryHelper.TrackEvent("LogDebug/Disabled Call");
		}

		/// <summary>
		/// Writes warnings, something not yet an error, but something to watch out for,
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		public override void LogWarning(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableWarning)
			{
				Log("Warning", message, container, exception, additionalData, metaData);
				TelemetryHelper.TrackEvent("LogWarning/Enabled Call");
			}
			else
				TelemetryHelper.TrackEvent("LogWarning/Disabled Call");
		}

		/// <summary>
		/// Writes errors, something handled and to be investigated,
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		public override void LogError(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableError)
			{
				Log("Error", message, container, exception, additionalData, metaData);
				TelemetryHelper.TrackEvent("LogError/Enabled Call");
			}
			else
				TelemetryHelper.TrackEvent("LogError/Disabled Call");
		}

		/// <summary>
		/// Writes fatal errors that have a detrimental effect on the system,
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		public override void LogFatalError(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableFatalError)
			{
				Log("Fatal", message, container, exception, additionalData, metaData);
				TelemetryHelper.TrackEvent("LogFatalError/Enabled Call");
			}
			else
				TelemetryHelper.TrackEvent("LogFatalError/Disabled Call");
		}

		#endregion

		/// <summary />
		protected virtual void Log(string level, string message, string container, Exception exception, IDictionary<string, object> additionalData, IDictionary<string, object> metaData)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(container))
				{
					var stackTrace = new StackTrace();
					StackFrame[] stackFrames = stackTrace.GetFrames();
					if (stackFrames != null)
					{
						foreach (StackFrame frame in stackFrames)
						{
							MethodBase method = frame.GetMethod();
							if (method.ReflectedType == null)
								continue;

							try
							{
								bool found = false;
								if (!ExclusionNamespaces.Any(@namespace => method.ReflectedType.FullName.StartsWith(@namespace)))
								{
									container = string.Format("{0}.{1}", method.ReflectedType.FullName, method.Name);
									found = true;
								}
								if (found)
									break;
							}
							catch
							{
								// Just move on
							}
						}
					}
				}
			}
			catch
			{
				// Just move on
			}

			var logInformation = new LogInformation
			{
				Level = level,
				Message = message,
				Container = container,
				Exception = JsonConvert.SerializeObject(exception),
				AdditionalData = additionalData,
				MetaData = JsonConvert.SerializeObject(metaData)
			};

			try
			{
				logInformation.CorrolationId = CorrelationIdHelper.GetCorrelationId();
			}
			catch (NullReferenceException)
			{
				logInformation.CorrolationId = Guid.Empty;
			}

			Log(() => PersistLog(logInformation), logInformation.Level, logInformation.Container);
		}

		/// <summary>
		/// Persists (or saves) the provided <paramref name="logInformation"></paramref> to the database
		/// </summary>
		/// <param name="logInformation">The <see cref="LogInformation"/> holding all the information you want to persist (save) to the database.</param>
		protected abstract void PersistLog(LogInformation logInformation);
	}
}