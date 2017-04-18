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
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using cdmdotnet.Performance;

namespace cdmdotnet.Logging
{
	/// <summary>
	/// Provides a set of methods that help you log events relating to the execution of your code.
	/// </summary>
	public abstract class VeryPrimitiveLogger : ILogger
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="StringLogger"/> class.
		/// </summary>
		protected VeryPrimitiveLogger(ILoggerSettings loggerSettings, ICorrelationIdHelper correlationIdHelper, ITelemetryHelper telemetryHelper)
		{
			LoggerSettings = loggerSettings;
			CorrelationIdHelper = correlationIdHelper;
			TelemetryHelper = telemetryHelper;
			if (TelemetryHelper == null)
			{
				if (loggerSettings.UseApplicationInsightTelemetryHelper)
					TelemetryHelper = (ITelemetryHelper)Activator.CreateInstanceFrom("cdmdotnet.Logging.Azure.ApplicationInsights.dll", "cdmdotnet.Logging.Azure.ApplicationInsights.TelemetryHelper").Unwrap();
				else
					TelemetryHelper = new NullTelemetryHelper();
			}
			ExclusionNamespaces = new List<string> { "cdmdotnet.Logging" };
			InprogressThreads = new List<Guid>();
		}

		/// <summary>
		/// The <see cref="ITelemetryHelper"/> for the instance, set during Instantiation
		/// </summary>
		protected ITelemetryHelper TelemetryHelper { get; private set; }

		/// <summary>
		/// The <see cref="ILoggerSettings"/> for the instance, set during Instantiation
		/// </summary>
		protected ILoggerSettings LoggerSettings { get; private set; }

		/// <summary>
		/// The <see cref="ICorrelationIdHelper"/> for the instance, set during Instantiation
		/// </summary>
		protected ICorrelationIdHelper CorrelationIdHelper { get; private set; }

		/// <summary>
		/// A list of namespaces to exclude when trying to automatically determine the container.
		/// </summary>
		protected IList<string> ExclusionNamespaces { get; private set; }

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

		internal IList<Guid> InprogressThreads { get; set; }

		/// <summary>
		/// Format a message based on the input parameters to be sent to <paramref name="logAction"></paramref>
		/// </summary>
		protected virtual void Log(Action logAction, string level, string container)
		{
			if (LoggerSettings.EnableThreadedLogging)
			{
				Guid threadGuid = Guid.NewGuid();
				InprogressThreads.Add(threadGuid);
				var tokenSource = new CancellationTokenSource();
				// Currently this doesn't need StartNewSafely as all thread based data is already collected and this would just slow things down.
				Task.Factory.StartNewSafely(() =>
				{
					using (tokenSource.Token.Register(Thread.CurrentThread.Abort))
					{
						try
						{
							PersistLogWithPerformanceTracking(logAction, level, container);
						}
						finally
						{
							InprogressThreads.Remove(threadGuid);
						}
					}
				}, tokenSource.Token);
			}
			else
				PersistLogWithPerformanceTracking(logAction, level, container);
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
				if (LoggerSettings.UsePerformanceCounters)
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
				}

				logAction();
			}
			catch (Exception exception)
			{
				Trace.TraceError("VeryPrimitiveLogger: Persisting log failed with the following exception:\r\n{0}\r\n{1}", exception.Message, exception.StackTrace);
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
		public virtual void Dispose()
		{
			if (EnableThreadedLoggingOutput)
				Trace.TraceInformation("VeryPrimitiveLogger: Dispose:: About to dispose.");

			int count = 0;
			while (InprogressThreads.Any())
			{
				Trace.TraceWarning("VeryPrimitiveLogger: Dispose:: Waiting for {0} logs to complete.", InprogressThreads.Count);
				Thread.Sleep(20);
				if (count++ > 1000)
					break;
			}

			if (InprogressThreads.Any())
				Trace.TraceWarning("VeryPrimitiveLogger: Dispose:: Disposed with {0} logs remaining.", InprogressThreads.Count);

			if (EnableThreadedLoggingOutput)
				Trace.TraceInformation("VeryPrimitiveLogger: Dispose:: Disposed.");
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