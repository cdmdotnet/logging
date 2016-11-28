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
using System.Threading;
using cdmdotnet.Logging.Configuration;
using cdmdotnet.Performance;
using Newtonsoft.Json;
using ThreadState = System.Threading.ThreadState;

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
			if (LoggerSettings.EnableThreadedLogging)
			{
				LoggingThreadsQueue = new Dictionary<Thread, LogInformation>();
				CurrentRunningThreadCountLock = new ReaderWriterLockSlim();
			}
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

		private IDictionary<Thread, LogInformation> LoggingThreadsQueue { get; set; }

		private Thread QueueThread { get; set; }

		/// <summary />
		protected bool IsDisposing { get; set; }

		/// <summary />
		protected bool IsDisposed { get; set; }

		private int CurrentRunningThreadCount { get; set; }

		private ReaderWriterLockSlim CurrentRunningThreadCountLock { get; set; }

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
		public void LogSensitive(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableSensitive)
				Log("Sensitive", message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// This is for logging general information, effectively the least amount of information you'd want to know about a system operation,
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// Don't abuse this as you will flood the logs as this would normally never turned off. Use <see cref="LogDebug"/> or <see cref="LogProgress"/> for reporting additional information.
		/// </summary>
		public void LogInfo(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableInfo)
				Log("Info", message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// Writes logging progress information such as "Process X is 24% done"
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		public void LogProgress(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableProgress)
				Log("Progress", message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// Writes diagnostic information 
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		public void LogDebug(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableDebug)
				Log("Debug", message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// Writes warnings, something not yet an error, but something to watch out for,
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		public void LogWarning(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableWarning)
				Log("Warning", message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// Writes errors, something handled and to be investigated,
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		public void LogError(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableError)
				Log("Error", message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// Writes fatal errors that have a detrimental effect on the system,
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		public void LogFatalError(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
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

			if (LoggerSettings.EnableThreadedLogging)
			{
				var persistingThread = new Thread(PersistLog)
				{
					Name = string.Format("Logger : {0}", logInformation.Level),
				};

				AddNewLogThread(persistingThread, logInformation);
			}
			else
				PersistLog(logInformation);
		}

		/// <summary>
		/// Helper method to create the ActionInfo object containing the info about the action that is getting called
		/// </summary>
		/// <returns>An ActionInfo object that contains all the information pertaining to what action is being executed</returns>
		private ActionInfo CreateActionInfo(string level, string container)
		{
			int processId = ConfigInfo.Value.ProcessId;
			String categoryName = ConfigInfo.Value.PerformanceCategoryName;

			ActionInfo info = new ActionInfo(processId, categoryName, "Logger", container, string.Empty, level, string.Empty);

			return info;
		}

		private void PersistLog(object parameters)
		{
			IPerformanceTracker performanceTracker = null;

			try
			{
				var logInformation = (LogInformation) parameters;

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

			if (LoggerSettings.EnableThreadedLogging)
				ShiftCurrentRunningThreadCount(false);
		}

		/// <summary>
		/// Persists (or saves) the provided <paramref name="logInformation"></paramref> to the database
		/// </summary>
		/// <param name="logInformation">The <see cref="LogInformation"/> holding all the information you want to persist (save) to the database.</param>
		protected abstract void PersistLog(LogInformation logInformation);

		private void PollLoggingQueue()
		{
			if (EnableThreadedLoggingOutput)
				Trace.TraceInformation("Logger: PollLoggingQueue:: {0}::: Getting keys from the queue.", Thread.CurrentThread.Name);
			IList<Thread> loggingThreads = LoggingThreadsQueue.Keys.ToList();

			int shutdownLoopCounter = 50;
			long loop = long.MinValue;

			while ((loggingThreads.Any() || shutdownLoopCounter > 0) && !IsDisposed)
			{
				// reset the counter as there is work to do;
				if (loggingThreads.Any())
					shutdownLoopCounter = 50;
				else
					shutdownLoopCounter--;
				foreach (Thread loggingThread in loggingThreads)
				{
					if (EnableThreadedLoggingOutput)
						Trace.TraceInformation("Logger: PollLoggingQueue:: {0}::: Thread name is '{1}' and state is '{2}'.", Thread.CurrentThread.Name, loggingThread.Name, loggingThread.ThreadState);
					switch (loggingThread.ThreadState)
					{
						case ThreadState.Stopped:
							LoggingThreadsQueue.Remove(loggingThread);
							if (EnableThreadedLoggingOutput)
								Trace.TraceInformation("Logger: PollLoggingQueue:: {0}::: Thread named '{1}' has been removed.", Thread.CurrentThread.Name, loggingThread.Name);
							break;
						case ThreadState.Unstarted:
							if (EnableThreadedLoggingOutput)
								Trace.TraceInformation("Logger: PollLoggingQueue:: {0}::: Entering read lock.", Thread.CurrentThread.Name);
							CurrentRunningThreadCountLock.EnterReadLock();
							bool shouldStartThread = CurrentRunningThreadCount < 5;
							if (EnableThreadedLoggingOutput)
								Trace.TraceInformation("Logger: PollLoggingQueue:: {0}::: Exiting read lock.", Thread.CurrentThread.Name);
							CurrentRunningThreadCountLock.ExitReadLock();

							if (EnableThreadedLoggingOutput)
								Trace.TraceInformation("Logger: PollLoggingQueue:: {0}::: Thread named '{1}' {2}.", Thread.CurrentThread.Name, loggingThread.Name, shouldStartThread ? "is being started" : "is still queued");
							if (shouldStartThread)
							{
								ShiftCurrentRunningThreadCount(true);
								loggingThread.Start(LoggingThreadsQueue[loggingThread]);
							}
							break;
					}
				}

				// sleep for a little bit
				if (EnableThreadedLoggingOutput)
					Trace.TraceInformation("Logger: PollLoggingQueue:: {0}::: About to sleep.", Thread.CurrentThread.Name);
				if (loop++ % 5 == 0)
					Thread.Yield();
				else
					Thread.Sleep(50);
				if (loop == long.MaxValue)
					loop = long.MinValue;
				if (EnableThreadedLoggingOutput)
					Trace.TraceInformation("Logger: PollLoggingQueue:: {0}::: Refreshing keys from the queue.", Thread.CurrentThread.Name);
				int retryCount = 0;
				while (retryCount < 10)
				{
					try
					{
						loggingThreads = LoggingThreadsQueue.Keys.ToList();
						break;
					}
					catch (Exception)
					{
						retryCount++;
					}
				}
			}
			if (EnableThreadedLoggingOutput)
				Trace.TraceInformation("Logger: PollLoggingQueue:: {0}::: Polling done.", Thread.CurrentThread.Name);
		}

		/// <summary>
		/// Increment or decrement the value of <see cref="CurrentRunningThreadCount"/> by 1
		/// </summary>
		/// <param name="increment">if true, then add 1, if false minus 1</param>
		private void ShiftCurrentRunningThreadCount(bool increment)
		{
			if (EnableThreadedLoggingOutput)
				Trace.TraceInformation("Logger: ShiftCurrentRunningThreadCount:: {0}::: Entering write lock.", Thread.CurrentThread.Name);
			CurrentRunningThreadCountLock.EnterWriteLock();

			if (increment)
			{
				if (EnableThreadedLoggingOutput)
					Trace.TraceInformation("Logger: ShiftCurrentRunningThreadCount:: {0}::: Incrementing count.", Thread.CurrentThread.Name);
				CurrentRunningThreadCount++;
			}
			else
			{
				if (EnableThreadedLoggingOutput)
					Trace.TraceInformation("Logger: ShiftCurrentRunningThreadCount:: {0}::: Decrementing count.", Thread.CurrentThread.Name);
				CurrentRunningThreadCount--;
			}

			if (EnableThreadedLoggingOutput)
				Trace.TraceInformation("Logger: ShiftCurrentRunningThreadCount:: {0}::: Exiting write lock.", Thread.CurrentThread.Name);
			CurrentRunningThreadCountLock.ExitWriteLock();
		}

		private void AddNewLogThread(Thread loggingThread, LogInformation logInformation)
		{
			if (IsDisposing)
				throw new InvalidOperationException("The logger is disposing.");

			if (IsDisposed)
				throw new InvalidOperationException("The logger is disposed.");

			LoggingThreadsQueue.Add(loggingThread, logInformation);

			CurrentRunningThreadCountLock.EnterUpgradeableReadLock();

			if (QueueThread == null || (QueueThread.ThreadState != ThreadState.Running && QueueThread.ThreadState != ThreadState.WaitSleepJoin))
			{
				CurrentRunningThreadCountLock.EnterWriteLock();
				// Recheck we actually need to do this in-case another thread did this for us
				if (QueueThread == null || (QueueThread.ThreadState != ThreadState.Running && QueueThread.ThreadState != ThreadState.WaitSleepJoin))
				{
					if (LoggerSettings.EnableThreadedLogging)
					{
						QueueThread = new Thread(PollLoggingQueue)
						{
							Name = GetQueueThreadName() ?? "Log queue polling"
						};

						QueueThread.Start();
					}
				}
				CurrentRunningThreadCountLock.ExitWriteLock();
				
			}

			CurrentRunningThreadCountLock.ExitUpgradeableReadLock();
		}

		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if (EnableThreadedLoggingOutput)
				Trace.TraceInformation("Logger: Dispose:: {0}::: About to dispose.", Thread.CurrentThread.Name);
			IsDisposing = true;

			// time to flush all remaining log operations
			if (LoggingThreadsQueue != null)
			{
				// This resolves any threaded issues where the collection can change on us.
				ICollection<Thread> keys = LoggingThreadsQueue.Keys;
				long loop = long.MinValue;

				while (keys.Any(loggingThread => loggingThread.ThreadState != ThreadState.Stopped))
				{
					IsDisposing = true;
					if (EnableThreadedLoggingOutput)
						Trace.TraceInformation("Logger: Dispose:: {0}::: About to sleep.", Thread.CurrentThread.Name);
					if (loop++ % 5 == 0)
						Thread.Yield();
					else
						Thread.Sleep(100);
					if (loop == long.MaxValue)
						loop = long.MinValue;
					// This resolves any threaded issues where the collection can change on us.
					keys = LoggingThreadsQueue.Keys;
				}
			}

			IsDisposed = true;
			IsDisposing = false;
			if (EnableThreadedLoggingOutput)
				Trace.TraceInformation("Logger: Dispose:: {0}::: Disposed.", Thread.CurrentThread.Name);
		}

		#endregion
	}
}