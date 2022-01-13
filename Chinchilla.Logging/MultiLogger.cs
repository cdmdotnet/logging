#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="Chinchilla Software Limited">
// // 	Copyright Chinchilla Software Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Chinchilla.Logging.Configuration;

namespace Chinchilla.Logging
{
	/// <summary>
	/// A <see cref="ILogger"/> that has several <see cref="ILogger"/> instances inside it that will get logs sent to.
	/// </summary>
	/// <remarks>
	/// Due to .NET Core no longer having access to SynchronizedCollection We're using Concurrent Dictionaries, which means a little messy work with values equalling keys.
	/// </remarks>
	public abstract class MultiLogger : ILogger
	{
		/// <summary>
		/// The internal loggers to log to. The keys MUST match the values
		/// </summary>
		protected IDictionary<ILogger, ILogger> Loggers { get; private set; }

		/// <summary>
		/// Instantiates a new instance of the <see cref="MultiLogger"/> class.
		/// </summary>
		protected MultiLogger()
		{
			Loggers = new ConcurrentDictionary<ILogger, ILogger>();
		}

		/// <summary>
		/// Puts the message to log into a set of <see cref="Task"/> one for each internal logger.
		/// </summary>
		/// <param name="logAction"></param>
		protected virtual void Log(Action<ILogger> logAction)
		{
			IDictionary<Task, Task> loggerTasks = new ConcurrentDictionary<Task, Task>();
			foreach (ILogger logger in Loggers.Keys)
			{
				ILogger log = logger;
				var task = Task.Factory.StartNewSafely(() =>
				{
					logAction(log);
				});
				loggerTasks.Add(task, task);
			}

			bool anyFailed = Task.Factory.ContinueWhenAll(loggerTasks.Keys.ToArray(), tasks =>
			{
				return tasks.Any(task => task.IsFaulted);
			}).Result;
			if (anyFailed)
				Trace.TraceError("One of the loggers failed.");
		}

		/// <summary>
		/// If <paramref name="container"/> is null or empty, generate a container name, otherwise return <paramref name="container"/>.
		/// </summary>
		/// <remarks>
		/// Almost perfectly copied from <see cref="VeryPrimitiveLogger"/>
		/// </remarks>
		protected virtual string UseOrBuildContainerName(string container)
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
								if (method.ReflectedType == null || string.IsNullOrWhiteSpace(method.ReflectedType.FullName))
									continue;
								if (!method.ReflectedType.FullName.StartsWith("Chinchilla.Logging"))
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
			return container;
		}

		#region Implementation of ILogger

		/// <summary>
		/// This is for logging sensitive information,
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// Depending on the implementation this won't be obscured or encrypted in anyway. Use this sparingly.
		/// </summary>
		public virtual void LogSensitive(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			Action<ILogger> logAction = logger =>
			{
				logger.LogSensitive(message, container, exception, additionalData, metaData);
			};

			Log(logAction);
		}

		/// <summary>
		/// This is for logging general information, effectively the least amount of information you'd want to know about a system operation,
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// Don't abuse this as you will flood the logs as this would normally never turned off. Use <see cref="ILogger.LogDebug"/> or <see cref="ILogger.LogProgress"/> for reporting additional information.
		/// </summary>
		public virtual void LogInfo(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			// this is needed here as jumping into a new thread/task would break this.
			container = UseOrBuildContainerName(container);

			Action<ILogger> logAction = logger =>
			{
				logger.LogInfo(message, container, exception, additionalData, metaData);
			};

			Log(logAction);
		}

		/// <summary>
		/// Writes logging progress information such as "Process X is 24% done"
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		public virtual void LogProgress(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			Action<ILogger> logAction = logger =>
			{
				logger.LogProgress(message, container, exception, additionalData, metaData);
			};

			Log(logAction);
		}

		/// <summary>
		/// Writes diagnostic information 
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		public virtual void LogDebug(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			Action<ILogger> logAction = logger =>
			{
				logger.LogDebug(message, container, exception, additionalData, metaData);
			};

			Log(logAction);
		}

		/// <summary>
		/// Writes warnings, something not yet an error, but something to watch out for,
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		public virtual void LogWarning(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			Action<ILogger> logAction = logger =>
			{
				logger.LogWarning(message, container, exception, additionalData, metaData);
			};

			Log(logAction);
		}

		/// <summary>
		/// Writes errors, something handled and to be investigated,
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		public virtual void LogError(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			Action<ILogger> logAction = logger =>
			{
				logger.LogError(message, container, exception, additionalData, metaData);
			};

			Log(logAction);
		}

		/// <summary>
		/// Writes fatal errors that have a detrimental effect on the system,
		/// to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		public virtual void LogFatalError(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			Action<ILogger> logAction = logger =>
			{
				logger.LogFatalError(message, container, exception, additionalData, metaData);
			};

			Log(logAction);
		}

		/// <summary>
		/// The <see cref="ILoggerSettings"/> for the instance, set during instantiation.
		/// </summary>
		public ILoggerSettings LoggerSettings { get { return Loggers.Keys.First().LoggerSettings; } }

		#endregion

		#region Implementation of IDisposable
#pragma warning disable CA1063 // Implement IDisposable Correctly

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public virtual void Dispose()
		{
			// Dispose of unmanaged resources.
			Dispose(true);
			// Suppress finalization.
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public virtual void Dispose(bool disposing)
		{
			IDictionary<Task, Task> loggerTasks = new ConcurrentDictionary<Task, Task>();
			if (disposing)
			{
				foreach (ILogger logger in Loggers.Keys)
				{
					ILogger log = logger;
					var task = Task.Factory.StartNewSafely(() =>
					{
						log.Dispose();
					});
					loggerTasks.Add(task, task);
				}
			}

			bool anyFailed = Task.Factory.ContinueWhenAll(loggerTasks.Keys.ToArray(), tasks =>
			{
				return tasks.Any(task => task.IsFaulted);
			}).Result;

			if (anyFailed)
				Trace.TraceError("One of the loggers failed to dispose.");
		}

#pragma warning restore CA1063 // Implement IDisposable Correctly
		#endregion

		/// <summary>
		/// Adds the provided <paramref name="logger"/> to <see cref="Loggers"/>.
		/// </summary>
		protected virtual void AddLogger(ILogger logger)
		{
			Loggers.Add(logger, logger);
			Type loggerType = typeof(VeryPrimitiveLogger);
			MethodInfo addExclusionNamespacemethod = loggerType.GetMethod("AddExclusionNamespace", BindingFlags.Instance |BindingFlags.NonPublic);
			addExclusionNamespacemethod.Invoke(logger, new object[] { "System.Threading" });
		}
	}

	/// <summary>
	/// A <see cref="ILogger"/> that has several <see cref="ILogger"/> instances inside it that will get logs sent to.
	/// </summary>
	public class MultiLogger<TLogger> : MultiLogger
		where TLogger : ILogger
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="MultiLogger"/> class.
		/// </summary>
		public MultiLogger(TLogger logger)
		{
			AddLogger(logger);
		}
	}

	/// <summary>
	/// A <see cref="ILogger"/> that has several <see cref="ILogger"/> instances inside it that will get logs sent to.
	/// </summary>
	public class MultiLogger<TLogger1, TLogger2> : MultiLogger
		where TLogger1 : ILogger
		where TLogger2 : ILogger
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="MultiLogger"/> class.
		/// </summary>
		public MultiLogger(TLogger1 logger1, TLogger2 logger2)
		{
			AddLogger(logger1);
			AddLogger(logger2);
		}
	}

	/// <summary>
	/// A <see cref="ILogger"/> that has several <see cref="ILogger"/> instances inside it that will get logs sent to.
	/// </summary>
	public class MultiLogger<TLogger1, TLogger2, TLogger3> : MultiLogger
		where TLogger1 : ILogger
		where TLogger2 : ILogger
		where TLogger3 : ILogger
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="MultiLogger"/> class.
		/// </summary>
		public MultiLogger(TLogger1 logger1, TLogger2 logger2, TLogger3 logger3)
		{
			AddLogger(logger1);
			AddLogger(logger2);
			AddLogger(logger3);
		}
	}

	/// <summary>
	/// A <see cref="ILogger"/> that has several <see cref="ILogger"/> instances inside it that will get logs sent to.
	/// </summary>
	public class MultiLogger<TLogger1, TLogger2, TLogger3, TLogger4> : MultiLogger
		where TLogger1 : ILogger
		where TLogger2 : ILogger
		where TLogger3 : ILogger
		where TLogger4 : ILogger
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="MultiLogger"/> class.
		/// </summary>
		public MultiLogger(TLogger1 logger1, TLogger2 logger2, TLogger3 logger3, TLogger4 logger4)
		{
			AddLogger(logger1);
			AddLogger(logger2);
			AddLogger(logger3);
			AddLogger(logger4);
		}
	}

	/// <summary>
	/// A <see cref="ILogger"/> that has several <see cref="ILogger"/> instances inside it that will get logs sent to.
	/// </summary>
	public class MultiLogger<TLogger1, TLogger2, TLogger3, TLogger4, TLogger5> : MultiLogger
		where TLogger1 : ILogger
		where TLogger2 : ILogger
		where TLogger3 : ILogger
		where TLogger4 : ILogger
		where TLogger5 : ILogger
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="MultiLogger"/> class.
		/// </summary>
		public MultiLogger(TLogger1 logger1, TLogger2 logger2, TLogger3 logger3, TLogger4 logger4, TLogger5 logger5)
		{
			AddLogger(logger1);
			AddLogger(logger2);
			AddLogger(logger3);
			AddLogger(logger4);
			AddLogger(logger5);
		}
	}
}