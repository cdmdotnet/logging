#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="Chinchilla Software Limited">
// // 	Copyright Chinchilla Software Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using Chinchilla.Logging;
using Chinchilla.Logging.Azure.Configuration;
using Chinchilla.Logging.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Test.Chinchilla.Logging;

#if NET472
#else
using Microsoft.Extensions.Configuration;
#endif

namespace Chinchilla.Logging.Tests.Integration
{
	[TestClass]
	public class VeryPrimitiveLoggerTests
	{
		[TestMethod]
		public async Task UseOrBuildContainerName_NoContainerName_AsyncSafeContainerName()
		{
			// Arrange
			var logger = new MockVeryPrimitiveLogger(new AzureLoggerSettingsConfiguration(
#if NET472
#else
				new ConfigurationBuilder()
					.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables()
				.Build()
#endif
				), new NullCorrelationIdHelper(), new NullTelemetryHelper());

			// Act
			string containerName = await logger.BuildContainerNameAsync();

			// Assert
			Assert.AreEqual("Test.Chinchilla.Logging.MockVeryPrimitiveLogger.BuildContainerNameAsync", containerName);
		}
	}
}

// This needs to be in a separate namespace to avoid exclusion rules.
namespace Test.Chinchilla.Logging
{
	internal class MockVeryPrimitiveLogger : VeryPrimitiveLogger
	{
		public MockVeryPrimitiveLogger(ILoggerSettings loggerSettings, ICorrelationIdHelper correlationIdHelper, ITelemetryHelper telemetryHelper)
			: base(loggerSettings, correlationIdHelper, telemetryHelper)
		{
		}

		public override void LogDebug(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			throw new NotImplementedException();
		}

		public override void LogError(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			throw new NotImplementedException();
		}

		public override void LogFatalError(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			throw new NotImplementedException();
		}

		public override void LogInfo(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			throw new NotImplementedException();
		}

		public override void LogProgress(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			throw new NotImplementedException();
		}

		public override void LogSensitive(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			throw new NotImplementedException();
		}

		public override void LogWarning(string message, string container = null, Exception exception = null, IDictionary<string, object> additionalData = null, IDictionary<string, object> metaData = null)
		{
			throw new NotImplementedException();
		}

		public string _UseOrBuildContainerName(string container)
		{
			return base.UseOrBuildContainerName(container);
		}

		public async Task<string> BuildContainerNameAsync()
		{
			string containerName = UseOrBuildContainerName(null);

			return await Task.FromResult(containerName);
		}
	}
}