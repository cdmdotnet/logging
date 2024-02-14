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
using Chinchilla.Logging.Azure.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

#if NET472
#else
using Microsoft.Extensions.Configuration;
#endif

namespace Chinchilla.Logging.Tests.Integration
{
	[TestClass]
	public class TelemetryHelperTests
	{
		[TestMethod]
		public void TrackException_SampleException_NoExpcetions()
		{
			// Arrange
#if NET472
#else
			var configurationBuilder = new ConfigurationBuilder()
				.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables()
				.Build();
#endif

			var telemetryHelper = new TelemetryHelper(new NullCorrelationIdHelper(), new AzureLoggerSettingsConfiguration(
#if NET472
#else
				configurationBuilder
#endif
				));

#pragma warning disable CS0618 // Type or member is obsolete
			TelemetryConfiguration.Active
#pragma warning restore CS0618 // Type or member is obsolete
				.ConnectionString =
#if NET472
				System.Configuration.ConfigurationManager.ConnectionStrings["ApplicationInsights"].ConnectionString
#else
				configurationBuilder.GetConnectionString("ApplicationInsights")
#endif
				;


			// Act
			telemetryHelper.TrackException(new ArgumentOutOfRangeException("Parameter1", "A Random message about parameter 1"));
			telemetryHelper.Flush();

			// Assert
		}
	}
}