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
using Chinchilla.Logging.Configuration;
using Newtonsoft.Json;

namespace Chinchilla.Logging
{
	/// <summary>
	/// Provides a set of methods that help you log events relating to the execution of your code.
	/// </summary>
	public abstract class PrimitiveLogger : VeryPrimitiveLogger
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="StringLogger"/> class.
		/// </summary>
		protected PrimitiveLogger(ILoggerSettings loggerSettings, ICorrelationIdHelper correlationIdHelper, ITelemetryHelper telemetryHelper)
			: base(loggerSettings, correlationIdHelper, telemetryHelper)
		{
		}

		/// <summary>
		/// Format a message based on the input parameters.
		/// </summary>
		protected virtual string GenerateLogMessage(string level, string message, string container, Exception exception, IDictionary<string, object> additionalData, IDictionary<string, object> metaData)
		{
			container = UseOrBuildContainerName(container);

			Guid corrolationId = Guid.Empty;
			try
			{
				corrolationId = CorrelationIdHelper.GetCorrelationId();
			}
			catch
			{
				// Default already set
			}

			string pattern = "[{0}] {1:r}:";
			if (corrolationId != Guid.Empty)
				pattern = "[{0}] [{7:N}] {1:r}:";
			if (!string.IsNullOrWhiteSpace(container))
				pattern = string.Concat(pattern, " {3}:: {2}");
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
				additionalData == null || !additionalData.Any() ? null : JsonConvert.SerializeObject(additionalData), // 8
				metaData == null || !metaData.Any() ? null : JsonConvert.SerializeObject(metaData) // 9
			);

			return messageToLog;
		}
	}
}