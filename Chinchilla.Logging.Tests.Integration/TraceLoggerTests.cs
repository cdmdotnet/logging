#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="Chinchilla Software Limited">
// // 	Copyright Chinchilla Software Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System;

using Chinchilla.Logging.Azure.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

#if NET472
#else
using Microsoft.Extensions.Configuration;
#endif

namespace Chinchilla.Logging.Tests.Integration
{
	[TestClass]
	public class TraceLoggerTests
	{
		public void LogMethod(string action)
		{
			// Arrange
			var traceLogger = new TraceLogger(new AzureLoggerSettingsConfiguration(
#if NET472
#else
				new ConfigurationBuilder()
					.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables()
				.Build()
#endif
				), new NullCorrelationIdHelper());

			Action<string, string, Exception, IDictionary<string, object>, IDictionary<string, object>> logMethod;
			switch (action?.ToLowerInvariant()?.Trim())
			{
				case "debug":
					logMethod = traceLogger.LogDebug;
					break;
				case "info":
					logMethod = traceLogger.LogInfo;
					break;
				case "progress":
					logMethod = traceLogger.LogProgress;
					break;
				case "warning":
					logMethod = traceLogger.LogWarning;
					break;
				case "sensitive":
					logMethod = traceLogger.LogSensitive;
					break;
				case "error":
					logMethod = traceLogger.LogError;
					break;
				case "fatalerror":
					logMethod = traceLogger.LogFatalError;
					break;
				default:
					throw new InvalidOperationException($"action {action} is not supported");
			}

			// Act
			logMethod("This is debug", "LogMethod", null, null, null);
		}

		[TestMethod]
		public void LogDebug_SimpleMessage_NoExpcetions()
		{
			// Arrange

			// Act
			LogMethod("Debug");

			// Assert
		}

		[TestMethod]
		public void LogInfo_SimpleMessage_NoExpcetions()
		{
			// Arrange

			// Act
			LogMethod("Info");

			// Assert
		}

		[TestMethod]
		public void LogProgress_SimpleMessage_NoExpcetions()
		{
			// Arrange

			// Act
			LogMethod("Progress");

			// Assert
		}

		[TestMethod]
		public void LogWarning_SimpleMessage_NoExpcetions()
		{
			// Arrange

			// Act
			LogMethod("Warning");

			// Assert
		}

		[TestMethod]
		public void LogSensitive_SimpleMessage_NoExpcetions()
		{
			// Arrange

			// Act
			LogMethod("Sensitive");

			// Assert
		}

		[TestMethod]
		public void LogError_SimpleMessage_NoExpcetions()
		{
			// Arrange

			// Act
			LogMethod("Error");

			// Assert
		}

		[TestMethod]
		public void LogFatalError_SimpleMessage_NoExpcetions()
		{
			// Arrange

			// Act
			LogMethod("FatalError");

			// Assert
		}
	}
}
