#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="cdmdotnet Limited">
// // 	Copyright cdmdotnet Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using cdmdotnet.Logging.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using cdmdotnet.Performance;
using Newtonsoft.Json;

namespace cdmdotnet.Logging
{
	/// <summary>
	/// Provides a set of methods that help you log events relating to the execution of your code.
	/// </summary>
	public abstract class PrimitiveLogger : ILogger
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="StringLogger"/> class.
		/// </summary>
		protected PrimitiveLogger(ILoggerSettings loggerSettings, ICorrelationIdHelper correlationIdHelper)
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

		private bool? _enableThreadedLoggingOutput;
		/// <summary />
		protected bool EnableThreadedLoggingOutput
		{
			get
			{
				if (_enableThreadedLoggingOutput == null)
					_enableThreadedLoggingOutput = LoggerSettings.EnableThreadedLoggingOutput;
				return _enableThreadedLoggingOutput.Value;
			}
		}

		/// <summary>
		/// Format a message based on the input parameters.
		/// </summary>
		protected virtual string GenerateLogMessage(string level, string message, string container, Exception exception, IDictionary<string, object> additionalData, IDictionary<string, object> metaData)
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
								if (!method.ReflectedType.FullName.StartsWith("cdmdotnet.Logging"))
								{
									container = string.Format("{0}.{1}", method.ReflectedType.FullName, method.Name);
									break;
								}
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


			Guid corrolationId = Guid.Empty;
			try
			{
				corrolationId = CorrelationIdHelper.GetCorrelationId();
			}
			catch
			{
				// Default already set
			}


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

			return messageToLog;
		}

		/// <summary>
		/// Helper method to create the ActionInfo object containing the info about the action that is getting called
		/// </summary>
		/// <returns>An ActionInfo object that contains all the information pertaining to what action is being executed</returns>
		protected virtual ActionInfo CreateActionInfo(string level, string container)
		{
			int processId = ConfigInfo.Value.ProcessId;
			String categoryName = ConfigInfo.Value.PerformanceCategoryName;

			ActionInfo info = new ActionInfo(processId, categoryName, "Logger", container, string.Empty, level, string.Empty);

			return info;
		}

		/// <summary>
		/// Added performance counters to persistence.
		/// </summary>
		protected virtual void PersistLogWithPerformanceTracking(Action logAction, string level, string container)
		{
			IPerformanceTracker performanceTracker = null;

			try
			{
				try
				{
					performanceTracker = new PerformanceTracker(CreateActionInfo(level, container));
					performanceTracker.ProcessActionStart();
				}
				catch (UnauthorizedAccessException) { }
				catch (Exception)
				{
					// Just move on
				}


				logAction();
			}
			catch (Exception exception)
			{
				Trace.TraceError("Persisting log failed with the following exception:\r\n{0}\r\n{1}", exception.Message, exception.StackTrace);
			}

			if (performanceTracker != null)
			{
				try
				{
					performanceTracker.ProcessActionComplete(false);
				}
				catch (UnauthorizedAccessException) { }
				catch (Exception)
				{
					// Just move on
				}
			}
		}

		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
		}

		#endregion

		#region Implementation of ILogger

		/// <summary>
		/// This is for logging sensitive information,
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// Depending on the implementation this won't be obscured or encrypted in anyway. Use this sparingly.
		/// </summary>
		public abstract void LogSensitive(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null);

		/// <summary>
		/// This is for logging general information, effectively the least amount of information you'd want to know about a system operation,
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// Don't abuse this as you will flood the logs as this would normally never turned off. Use <see cref="ILogger.LogDebug"/> or <see cref="ILogger.LogProgress"/> for reporting additional information.
		/// </summary>
		public abstract void LogInfo(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null);

		/// <summary>
		/// Writes logging progress information such as "Process X is 24% done"
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		public abstract void LogProgress(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null);

		/// <summary>
		/// Writes diagnostic information 
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		public abstract void LogDebug(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null);

		/// <summary>
		/// Writes warnings, something not yet an error, but something to watch out for,
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		public abstract void LogWarning(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null);

		/// <summary>
		/// Writes errors, something handled and to be investigated,
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		public abstract void LogError(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null);

		/// <summary>
		/// Writes fatal errors that have a detrimental effect on the system,
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		public abstract void LogFatalError(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null);

		#endregion
	}
}