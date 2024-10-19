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
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Chinchilla.Logging.Configuration;

namespace Chinchilla.Logging
{
	/// <summary>
	/// Provides a set of methods that help you log events relating to the execution of your code.
	/// </summary>
	/// <remarks>
	/// Due to .NET Core no longer having access to SynchronizedCollection We're using Concurrent Dictionaries, which means a little messy work with values equalling keys.
	/// </remarks>
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
				{
#if NETSTANDARD2_0
					TelemetryHelper = (ITelemetryHelper)CreateInstanceFrom("Chinchilla.Logging.Azure.ApplicationInsights.dll", "Chinchilla.Logging.Azure.ApplicationInsights.TelemetryHelper");
#else
					TelemetryHelper = (ITelemetryHelper)Activator.CreateInstanceFrom("Chinchilla.Logging.Azure.ApplicationInsights.dll", "Chinchilla.Logging.Azure.ApplicationInsights.TelemetryHelper").Unwrap();
#endif
				}
				else
					TelemetryHelper = new NullTelemetryHelper();
			}
			ExclusionNamespaces = new ConcurrentDictionary<string, string>();
			AddExclusionNamespace("Chinchilla.Logging");
			AddExclusionNamespace("System.Runtime.CompilerServices.Async");
			AddExclusionNamespace("System.Runtime.CompilerServices.TaskAwaiter");
			AddExclusionNamespace("System.Threading.ExecutionContext");
			AddExclusionNamespace("System.Threading.Tasks.AwaitTaskContinuation");
			AddExclusionNamespace("System.Threading.Tasks.Task");
			InprogressThreads = new ConcurrentDictionary<Guid, string>();
			TaskRelatedMethodNames = new List<string>
			{
				"MoveNext",
				"Start"
			};
			ContainerNameMatcher = new Regex("^(.)+?>", RegexOptions.IgnoreCase);
		}

#if NETSTANDARD2_0
		private static object CreateInstanceFrom(string assemblyFile, string typeName)
		{
			Assembly assembly = Assembly.LoadFrom(assemblyFile);
			Type t = assembly.GetType(typeName);

			return Activator.CreateInstance(t);
		}
#endif
		/// <summary>
		/// The <see cref="ITelemetryHelper"/> for the instance, set during Instantiation
		/// </summary>
		protected ITelemetryHelper TelemetryHelper { get; private set; }

		/// <summary>
		/// The <see cref="ILoggerSettings"/> for the instance, set during Instantiation
		/// </summary>
		public ILoggerSettings LoggerSettings { get; private set; }

		/// <summary>
		/// The <see cref="ICorrelationIdHelper"/> for the instance, set during Instantiation
		/// </summary>
		protected ICorrelationIdHelper CorrelationIdHelper { get; private set; }

		/// <summary>
		/// A list of namespaces to exclude when trying to automatically determine the container. The key and the value MUST match each other.
		/// </summary>
		protected IDictionary<string, string> ExclusionNamespaces { get; private set; }

		private IList<string> TaskRelatedMethodNames { get; }

		private Regex ContainerNameMatcher { get; }

		/// <summary>
		/// Adds the provided <paramref name="namespaces"/> to <see cref="ExclusionNamespaces"/>.
		/// </summary>
		protected virtual void AddExclusionNamespace(params string[] @namespaces)
		{
			foreach(string @namespace in namespaces)
				ExclusionNamespaces.Add(@namespace, @namespace);
		}

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

		internal IDictionary<Guid, string> InprogressThreads { get; set; }

		/// <summary>
		/// Format a message based on the input parameters to be sent to <paramref name="logAction"></paramref>
		/// </summary>
		protected virtual void Log(Action logAction, string level, string container)
		{
			if (GetSetting(container, containerLoggerSettings => containerLoggerSettings.EnableThreadedLogging, LoggerSettings.EnableThreadedLogging))
			{
				Guid threadGuid = Guid.NewGuid();
				InprogressThreads.Add(threadGuid, string.Empty);
				var tokenSource = new CancellationTokenSource();
				// Currently this doesn't need StartNewSafely as all thread based data is already collected and this would just slow things down.
				Task.Factory.StartNewSafely(() =>
				{
#if NET40
					using (tokenSource.Token.Register(Thread.CurrentThread.Abort))
#endif
					{
						try
						{
							logAction();
						}
						finally
						{
							InprogressThreads.Remove(threadGuid);
						}
					}
				}, tokenSource.Token);
			}
			else
				logAction();
		}

		/// <summary>
		/// If <paramref name="container"/> is null or empty, generate a container name, otherwise return <paramref name="container"/>.
		/// </summary>
		/// <remarks>
		/// Almost perfectly copied to <see cref="MultiLogger"/>
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
								if (ExclusionNamespaces.All(@namespace => !method.ReflectedType.FullName.StartsWith(@namespace.Key)))
								{
									Match match;
									if (method.DeclaringType.IsSealed && method.DeclaringType.IsNestedPrivate && method.DeclaringType.IsAutoLayout && TaskRelatedMethodNames.Contains(method.Name) && (match = ContainerNameMatcher.Match(method.ReflectedType.FullName)).Success)
									{
										container = match.Value.Substring(0, match.Value.Length - 1).Replace("+<", "\\");
										found = true;
									}
									else
									{
										container = $"{method.ReflectedType.FullName}\\{method.Name}";
										found = true;
									}
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

		/// <summary>
		/// Get a boolean setting trying a <see cref="IContainerLoggerSettings"/> first then <see cref="LoggerSettings"/>
		/// </summary>
		protected virtual bool GetSetting(string container, Func<IContainerLoggerSettings, Func<string, bool>> setting, bool defaultValue)
		{
			var containerLoggerSettings = LoggerSettings as IContainerLoggerSettings;
			return ((containerLoggerSettings == null ? defaultValue : setting(containerLoggerSettings)(container)));
		}

		/// <summary>
		/// Get a string setting trying a <see cref="IContainerLoggerSettings"/> first then <see cref="LoggerSettings"/>
		/// </summary>
		protected virtual string GetSetting(string container, Func<IContainerLoggerSettings, Func<string, string>> setting, string defaultValue)
		{
			var containerLoggerSettings = LoggerSettings as IContainerLoggerSettings;
			return containerLoggerSettings == null ? defaultValue : setting(containerLoggerSettings)(container);
		}

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
			if (EnableThreadedLoggingOutput)
				Trace.TraceInformation("VeryPrimitiveLogger: Dispose:: About to dispose.");

			if (disposing)
			{
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
			}

			if (EnableThreadedLoggingOutput)
				Trace.TraceInformation("VeryPrimitiveLogger: Dispose:: Disposed.");
		}

#pragma warning restore CA1063 // Implement IDisposable Correctly
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