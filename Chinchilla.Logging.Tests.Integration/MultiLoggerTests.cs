#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="Chinchilla Software Limited">
// // 	Copyright Chinchilla Software Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;

using Chinchilla.Logging.Azure;
using Chinchilla.Logging.Azure.Configuration;
using Chinchilla.Logging.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if NET472
#else
using Microsoft.Extensions.Configuration;
#endif

namespace Chinchilla.Logging.Tests.Integration
{
	[TestClass]
	public class MultiLoggerTests
	{
		[TestMethod]
		public void Constructor_ConsoleLoggers_AdditionalExclusionNAmespacesAdded()
		{
			// Arrange
			var logger1 = new TestConsoleLogger(new AzureLoggerSettingsConfiguration(
#if NET472
#else
					new ConfigurationBuilder()
						.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
						.AddEnvironmentVariables()
						.Build()
#endif
					), new NullCorrelationIdHelper(), new NullTelemetryHelper()
				);
			var logger2 = new TestConsoleLogger(new AzureLoggerSettingsConfiguration(
#if NET472
#else
					new ConfigurationBuilder()
						.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
						.AddEnvironmentVariables()
						.Build()
#endif
					), new NullCorrelationIdHelper(), new NullTelemetryHelper()
				);
			var logger = new MultiLogger<ConsoleLogger, ConsoleLogger>(logger1, logger2);

			// Assert
			Assert.IsTrue(logger1.GetExclusionNamespace().Contains("System.Threading.Tasks"), "Logger 1 is missing the expcetd exclusion namespace");
			Assert.IsTrue(logger2.GetExclusionNamespace().Contains("System.Threading.Tasks"), "Logger 2 is missing the expcetd exclusion namespace");
		}

		[TestMethod]
		public void Constructor_ConsoleLoggers_CorrectContainNameGenerated()
		{
			// Arrange
			var logger1 = new TestConsoleLogger(new AzureLoggerSettingsConfiguration(
#if NET472
#else
					new ConfigurationBuilder()
						.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
						.AddEnvironmentVariables()
						.Build()
#endif
					), new NullCorrelationIdHelper(), new NullTelemetryHelper()
				);
			var logger2 = new TestConsoleLogger(new AzureLoggerSettingsConfiguration(
#if NET472
#else
					new ConfigurationBuilder()
						.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
						.AddEnvironmentVariables()
						.Build()
#endif
					), new NullCorrelationIdHelper(), new NullTelemetryHelper()
				);
			var logger = new MultiLogger<ConsoleLogger, ConsoleLogger>(logger1, logger2);

			// Act
			new NamespaceSensitiveTests.LogTests().LogInfo(logger);

			// Assert
			Assert.IsTrue(logger1.GeneratedContainerNames.All(x => x == "NamespaceSensitiveTests.LogTests.LogInfo"), "Logger 1 did not generate the correct container name");
			Assert.IsTrue(logger2.GeneratedContainerNames.All(x => x == "NamespaceSensitiveTests.LogTests.LogInfo"), "Logger 2 did not generate the correct container name");
		}
	}
}

namespace Chinchilla.Logging
{
	class TestConsoleLogger : ConsoleLogger
	{
		public bool IsLogPersistingEnabled { get; set; }

		public IList<string> GeneratedContainerNames { get; set; }

		public TestConsoleLogger(ILoggerSettings loggerSettings, ICorrelationIdHelper correlationIdHelper, ITelemetryHelper telemetryHelper = null, bool isLogPersistingEnabled = true)
			: base(loggerSettings, correlationIdHelper, telemetryHelper)
		{
			IsLogPersistingEnabled = isLogPersistingEnabled;
			GeneratedContainerNames = new List<string>();
		}

		protected override string UseOrBuildContainerName(string container)
		{
			string generatedContainerName = base.UseOrBuildContainerName(container);
			GeneratedContainerNames.Add(generatedContainerName);
			return generatedContainerName;
		}

		protected override void Log(Action logAction, string level, string container)
		{
			if (IsLogPersistingEnabled)
				base.Log(logAction, level, container);
		}

		public IEnumerable<string> GetExclusionNamespace()
		{
			return ExclusionNamespaces.Keys;
		}
	}
}

namespace NamespaceSensitiveTests
{
	using Chinchilla.Logging;

	class LogTests
	{
		public void LogInfo(ILogger logger)
		{
			logger.LogInfo("Test Message");
		}
	}
}