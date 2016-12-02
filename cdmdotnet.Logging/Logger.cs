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
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using cdmdotnet.Logging.Configuration;
using cdmdotnet.Performance;
using Newtonsoft.Json;

namespace cdmdotnet.Logging
{
	/// <summary>
	/// Provides a set of methods that help you log events relating to the execution of your code.
	/// </summary>
	public abstract class Logger : ILogger
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="Logger"/> class preparing the required thread pool polling if <see cref="ILoggerSettings.EnableThreadedLogging"/> is set to true.
		/// </summary>
		protected Logger(ILoggerSettings loggerSettings, ICorrelationIdHelper correlationIdHelper)
		{
			LoggerSettings = loggerSettings;
			CorrelationIdHelper = correlationIdHelper;

			// CreateTable();
		}

		/// <summary />
		protected abstract string GetQueueThreadName();

		/// <summary>
		/// The <see cref="ILoggerSettings"/> for the instance, set during Instantiation
		/// </summary>
		protected ILoggerSettings LoggerSettings { get; private set; }

		/// <summary>
		/// The <see cref="ICorrelationIdHelper"/> for the instance, set during Instantiation
		/// </summary>
		protected ICorrelationIdHelper CorrelationIdHelper { get; private set; }

		/// <summary />
		protected bool IsDisposing { get; set; }

		/// <summary />
		protected bool IsDisposed { get; set; }

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

		#region Implementation of ILog

		/// <summary>
		/// This is for logging sensitive information,
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// Depending on the implementation this won't be obscured or encrypted in anyway. Use this sparingly.
		/// </summary>
		public virtual void LogSensitive(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableSensitive)
				Log("Sensitive", message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// This is for logging general information, effectively the least amount of information you'd want to know about a system operation,
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// Don't abuse this as you will flood the logs as this would normally never turned off. Use <see cref="LogDebug"/> or <see cref="LogProgress"/> for reporting additional information.
		/// </summary>
		public virtual void LogInfo(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableInfo)
				Log("Info", message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// Writes logging progress information such as "Process X is 24% done"
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		public virtual void LogProgress(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableProgress)
				Log("Progress", message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// Writes diagnostic information 
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		public virtual void LogDebug(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableDebug)
				Log("Debug", message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// Writes warnings, something not yet an error, but something to watch out for,
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		public virtual void LogWarning(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableWarning)
				Log("Warning", message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// Writes errors, something handled and to be investigated,
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		public virtual void LogError(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableError)
				Log("Error", message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// Writes fatal errors that have a detrimental effect on the system,
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		public virtual void LogFatalError(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableFatalError)
				Log("Fatal", message, container, exception, additionalData, metaData);
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
								if (!method.ReflectedType.FullName.StartsWith(GetType().Namespace.Replace(".Sql", null)))
								{
									container = string.Format("{0}.{1}", method.ReflectedType.FullName, method.Name);
									break;
								}
							}
							catch { }
						}
					}
				}
			}
			catch { }

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

			if (LoggerSettings.EnableThreadedLogging)
			{
				var tokenSource = new CancellationTokenSource();
				Task.Factory.StartNew(() =>
				{
					using (tokenSource.Token.Register(Thread.CurrentThread.Abort))
					{
						PersistLogWithPerformanceTracking(logInformation);
					}
				}, tokenSource.Token);
			}
			else
				PersistLogWithPerformanceTracking(logInformation);
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
		protected virtual void PersistLogWithPerformanceTracking(LogInformation logInformation)
		{
			IPerformanceTracker performanceTracker = null;

			try
			{
				try
				{
					performanceTracker = new PerformanceTracker(CreateActionInfo(logInformation.Level, logInformation.Container));
					performanceTracker.ProcessActionStart();
				}
				catch (UnauthorizedAccessException) { }
				catch (Exception) { }

				PersistLog(logInformation);
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
				catch (Exception) { }
			}
		}

		/// <summary>
		/// Persists (or saves) the provided <paramref name="logInformation"></paramref> to the database
		/// </summary>
		/// <param name="logInformation">The <see cref="LogInformation"/> holding all the information you want to persist (save) to the database.</param>
		protected abstract void PersistLog(LogInformation logInformation);

		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public virtual void Dispose()
		{
			if (EnableThreadedLoggingOutput)
				Trace.TraceInformation("Logger: Dispose:: {0}::: About to dispose.", Thread.CurrentThread.Name);

			if (EnableThreadedLoggingOutput)
				Trace.TraceInformation("Logger: Dispose:: {0}::: Disposed.", Thread.CurrentThread.Name);
		}

		#endregion
	}
}