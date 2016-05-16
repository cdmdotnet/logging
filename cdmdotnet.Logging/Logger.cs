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

				var queueThread = new Thread(PollLoggingQueue)
				{
					Name = GetQueueThreadName() ?? "Log queue polling"
				};

				queueThread.Start();
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
		/// Writes an informational message to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		public void LogInfo(string message, string container = null, Exception exception = null)
		{
			if (LoggerSettings.EnableInfo)
				Log("Info", message, container, exception);
		}

		/// <summary>
		/// Writes a debugging message to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		public void LogDebug(string message, string container = null, Exception exception = null)
		{
			if (LoggerSettings.EnableDebug)
				Log("Debug", message, container, exception);
		}

		/// <summary>
		/// Writes a warning message to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		public void LogWarning(string message, string container = null, Exception exception = null)
		{
			if (LoggerSettings.EnableWarning)
				Log("Warning", message, container, exception);
		}

		/// <summary>
		/// Writes an error message to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		public void LogError(string message, string container = null, Exception exception = null)
		{
			if (LoggerSettings.EnableError)
				Log("Error", message, container, exception);
		}

		/// <summary>
		/// Writes a fatal error message to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		public void LogFatalError(string message, string container = null, Exception exception = null)
		{
			if (LoggerSettings.EnableFatalError)
				Log("Fatal", message, container, exception);
		}

		#endregion

		/// <summary />
		protected virtual void Log(string level, string message, string container, Exception exception)
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
				Exception = JsonConvert.SerializeObject(exception)
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

			while (loggingThreads.Any() || !IsDisposed)
			{
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
				Thread.Sleep(50);
				if (EnableThreadedLoggingOutput)
					Trace.TraceInformation("Logger: PollLoggingQueue:: {0}::: Refreshing keys from the queue.", Thread.CurrentThread.Name);
				loggingThreads = LoggingThreadsQueue.Keys.ToList();
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
				while (keys.Any(loggingThread => loggingThread.ThreadState != ThreadState.Stopped))
				{
					IsDisposing = true;
					if (EnableThreadedLoggingOutput)
						Trace.TraceInformation("Logger: Dispose:: {0}::: About to sleep.", Thread.CurrentThread.Name);
					Thread.Sleep(100);
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