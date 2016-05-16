#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="cdmdotnet Limited">
// // 	Copyright cdmdotnet Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System;
using System.Diagnostics;
using System.Reflection;
using cdmdotnet.Logging.Configuration;

namespace cdmdotnet.Logging
{
	public class ConsoleLogger : ILogger
	{
		public ConsoleLogger(ILoggerSettings loggerSettings, ICorrelationIdHelper correlationIdHelper)
		{
			LoggerSettings = loggerSettings;
			CorrelationIdHelper = correlationIdHelper;
		}

		protected ILoggerSettings LoggerSettings { get; private set; }

		protected ICorrelationIdHelper CorrelationIdHelper { get; private set; }

		#region Implementation of ILog

		public void LogInfo(string message, string container = null, Exception exception = null)
		{
			if (LoggerSettings.EnableInfo)
				Log("Info", ConsoleColor.Gray, message, container, exception);
		}

		public void LogDebug(string message, string container = null, Exception exception = null)
		{
			if (LoggerSettings.EnableDebug)
				Log("Debug", ConsoleColor.Blue, message, container, exception);
		}

		public void LogWarning(string message, string container = null, Exception exception = null)
		{
			if (LoggerSettings.EnableWarning)
				Log("Warning", ConsoleColor.DarkYellow, message, container, exception);
		}

		public void LogError(string message, string container = null, Exception exception = null)
		{
			if (LoggerSettings.EnableError)
				Log("Error", ConsoleColor.DarkRed, message, container, exception);
		}

		public void LogFatalError(string message, string container = null, Exception exception = null)
		{
			if (LoggerSettings.EnableFatalError)
				Log("Fatal", ConsoleColor.Red, message, container, exception);
		}

		#endregion

		protected virtual void Log(string level, ConsoleColor foregroundColor, string message, string container, Exception exception)
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
			string messageToLog = string.Format(pattern, level, // 0
				DateTime.Now, // 1
				message, // 2
				container, // 3
				exception, // 4
				exception == null ? null : exception.Message, // 5
				exception == null ? null : exception.StackTrace, // 6
				corrolationId // 7
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