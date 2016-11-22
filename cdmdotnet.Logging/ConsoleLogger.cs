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
using cdmdotnet.Logging.Configuration;
using Newtonsoft.Json;

namespace cdmdotnet.Logging
{
	/// <summary>
	/// Provides a set of methods that help you log events relating to the execution of your code outputting to the <see cref="Console"/>
	/// </summary>
	public class ConsoleLogger : ILogger
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="ConsoleLogger"/> class.
		/// </summary>
		public ConsoleLogger(ILoggerSettings loggerSettings, ICorrelationIdHelper correlationIdHelper)
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

		#region Implementation of ILogger

		/// <summary>
		/// This is for logging general information, effectively the least amount of information you'd want to know about a system operation,
		/// to the <see cref="Console"/> in <see cref="ConsoleColor.Gray"/>
		/// Don't abuse this as you will flood the logs as this would normally never turned off. Use <see cref="LogDebug"/> or <see cref="LogProgress"/> for reporting additional information.
		/// </summary>
		public void LogInfo(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableInfo)
				Log("Info", ConsoleColor.Gray, message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// Writes logging progress information such as "Process X is 24% done"
		/// to the <see cref="Console"/> in <see cref="ConsoleColor.Gray"/>
		/// </summary>
		public void LogProgress(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableProgress)
				Log("Progress", ConsoleColor.Gray, message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// Writes diagnostic information 
		/// to the <see cref="Console"/> in <see cref="ConsoleColor.Blue"/>
		/// </summary>
		public void LogDebug(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableDebug)
				Log("Debug", ConsoleColor.Blue, message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// Writes warnings, something not yet an error, but something to watch out for,
		/// to the <see cref="Console"/> in <see cref="ConsoleColor.DarkYellow"/>
		/// </summary>
		public void LogWarning(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableWarning)
				Log("Warning", ConsoleColor.DarkYellow, message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// Writes errors, something handled and to be investigated,
		/// to the <see cref="Console"/> in <see cref="ConsoleColor.DarkRed"/>
		/// </summary>
		public void LogError(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableError)
				Log("Error", ConsoleColor.DarkRed, message, container, exception, additionalData, metaData);
		}

		/// <summary>
		/// Writes fatal errors that have a detrimental effect on the system,
		/// to the <see cref="Console"/> in <see cref="ConsoleColor.Red"/>
		/// </summary>
		public void LogFatalError(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			if (LoggerSettings.EnableFatalError)
				Log("Fatal", ConsoleColor.Red, message, container, exception, additionalData, metaData);
		}

		#endregion

		/// <summary>
		/// Format a message based on the input parameters to be sent to the <see cref="Console"/>
		/// </summary>
		protected virtual void Log(string level, ConsoleColor foregroundColor, string message, string container, Exception exception, IDictionary<string, object> additionalData, IDictionary<string, object> metaData)
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

			Guid corrolationId = Guid.Empty;
			try
			{
				corrolationId = CorrelationIdHelper.GetCorrelationId();
			}
			catch { }

			string pattern = "[{0}] {1:r}:";
			if (corrolationId != Guid.Empty)
				pattern = "[{0}] [{7:N}] {1:r}:";
			if (!string.IsNullOrWhiteSpace(container))
				pattern = string.Concat(pattern, " {3}:: {2}");
			else
				pattern = string.Concat(pattern, " {2}");
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

			ConsoleColor originalColour = Console.ForegroundColor;
			Console.ForegroundColor = foregroundColor;
			Console.WriteLine(messageToLog);
			Console.ForegroundColor = originalColour;
		}

		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
		}

		#endregion
	}
}