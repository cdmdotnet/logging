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
using System.Threading.Tasks;

namespace cdmdotnet.Logging
{
	/// <summary>
	/// A <see cref="ILogger"/> that has several <see cref="ILogger"/> instances inside it that will get logs sent to.
	/// </summary>
	public abstract class MultiLogger : ILogger
	{
		/// <summary>
		/// The internal loggers to log to.
		/// </summary>
		protected IList<ILogger> Loggers { get; private set; }

		/// <summary>
		/// Instantiates a new instance of the <see cref="MultiLogger"/> class.
		/// </summary>
		protected MultiLogger()
		{
			Loggers = new SynchronizedCollection<ILogger>();
		}

		/// <summary>
		/// Puts the message to log into a set of <see cref="Task"/> one for each internal logger.
		/// </summary>
		/// <param name="logAction"></param>
		protected virtual void Log(Action<ILogger> logAction)
		{
			IList<Task> loggerTasks = new SynchronizedCollection<Task>();
			foreach (ILogger logger in Loggers)
			{
				ILogger log = logger;
				var task = Task.Factory.StartNewSafely(() =>
				{
					logAction(log);
				});
				loggerTasks.Add(task);
			}

			bool anyFailed = Task.Factory.ContinueWhenAll(loggerTasks.ToArray(), tasks =>
			{
				return tasks.Any(task => task.IsFaulted);
			}).Result;
			if (anyFailed)
				Trace.TraceError("One of the loggers failed.");
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

		#endregion

		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			IList<Task> loggerTasks = new SynchronizedCollection<Task>();
			foreach (ILogger logger in Loggers)
			{
				ILogger log = logger;
				var task = Task.Factory.StartNewSafely(() =>
				{
					log.Dispose();
				});
				loggerTasks.Add(task);
			}

			bool anyFailed = Task.Factory.ContinueWhenAll(loggerTasks.ToArray(), tasks =>
			{
				return tasks.Any(task => task.IsFaulted);
			}).Result;
			if (anyFailed)
				Trace.TraceError("One of the loggers failed to dispose.");
		}

		#endregion
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
			Loggers.Add(logger);
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
			Loggers.Add(logger1);
			Loggers.Add(logger2);
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
			Loggers.Add(logger1);
			Loggers.Add(logger2);
			Loggers.Add(logger3);
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
			Loggers.Add(logger1);
			Loggers.Add(logger2);
			Loggers.Add(logger3);
			Loggers.Add(logger4);
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
			Loggers.Add(logger1);
			Loggers.Add(logger2);
			Loggers.Add(logger3);
			Loggers.Add(logger4);
			Loggers.Add(logger5);
		}
	}
}