#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="Chinchilla Software Limited">
// // 	Copyright Chinchilla Software Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System.Diagnostics;
using Chinchilla.Logging.Configuration;

namespace Chinchilla.Logging
{
	/// <summary>
	/// Provides a set of methods that help you log events relating to the execution of your code outputting to <see cref="Trace"/>
	/// </summary>
	public class TraceLogger : StringLogger
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="TraceLogger"/> class.
		/// </summary>
		public TraceLogger(ILoggerSettings loggerSettings, ICorrelationIdHelper correlationIdHelper, ITelemetryHelper telemetryHelper = null)
			: base(loggerSettings, correlationIdHelper, telemetryHelper)
		{
		}

		/// <summary>
		/// Writes sensitive information to <see cref="Trace.TraceInformation(string)"/>
		/// </summary>
		protected override void LogSensitiveString(string message)
		{
			Trace.TraceInformation(message);
		}

		/// <summary>
		/// Writes an informational message to <see cref="Trace.TraceInformation(string)"/>
		/// </summary>
		protected override void LogInfoString(string message)
		{
			Trace.TraceInformation(message);
		}

		/// <summary>
		/// Writes logging progress information such as "Process X is 24% done" to <see cref="Trace.TraceInformation(string)"/>
		/// </summary>
		protected override void LogProgressString(string message)
		{
			Trace.TraceInformation(message);
		}

		/// <summary>
		/// Writes a debugging message to <see cref="Trace.TraceInformation(string)"/>
		/// </summary>
		protected override void LogDebugString(string message)
		{
			Trace.TraceInformation(message);
		}

		/// <summary>
		/// Writes a warning message to <see cref="Trace.TraceWarning(string)"/>
		/// </summary>
		protected override void LogWarningString(string message)
		{
			Trace.TraceWarning(message);
		}

		/// <summary>
		/// Writes an error message to <see cref="Trace.TraceError(string)"/>
		/// </summary>
		protected override void LogErrorString(string message)
		{
			Trace.TraceError(message);
		}

		/// <summary>
		/// Writes a fatal error message to <see cref="Trace.TraceError(string)"/>
		/// </summary>
		protected override void LogFatalErrorString(string message)
		{
			Trace.TraceError(message);
		}
	}
}