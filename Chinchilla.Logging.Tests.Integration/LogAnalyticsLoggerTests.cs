#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="Chinchilla Software Limited">
// // 	Copyright Chinchilla Software Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System;

using Chinchilla.Logging.Azure;
using Chinchilla.Logging.Azure.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if NET472
#else
using Microsoft.Extensions.Configuration;
#endif

namespace Chinchilla.Logging.Tests.Integration
{
	[TestClass]
	public class LogAnalyticsLoggerTests
	{
//		[TestMethod]
		public void LogError_SampleMessage_NoExpcetions()
		{
			// Arrange
			var logger = new LogAnalyticsLogger(new AzureLoggerSettingsConfiguration(
#if NET472
#else
				new ConfigurationBuilder()
					.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
					.AddEnvironmentVariables()
					.Build()
#endif
				), new NullCorrelationIdHelper(), new NullTelemetryHelper());

			// Act
			logger.LogError("A Test", exception: new ArgumentOutOfRangeException("Parameter1", "A Random message about parameter 1"));

			// Assert
		}
	}
}