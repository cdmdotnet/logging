#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="Chinchilla Software Limited">
// // 	Copyright Chinchilla Software Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using Chinchilla.Logging.Configuration;
using Serilogger = Serilog.Log;

namespace Chinchilla.Logging.Serilog
{
	/// <summary>
	/// Provides a set of methods that help you log events relating to the execution of your code outputting to a configured <see cref="Serilogger"/>.
	/// </summary>
	public class SerilogLogger : StringLogger
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="SerilogLogger"/> class.
		/// </summary>
		public SerilogLogger(ILoggerSettings loggerSettings, ICorrelationIdHelper correlationIdHelper, ITelemetryHelper telemetryHelper = null)
			: base(loggerSettings, correlationIdHelper, telemetryHelper)
		{
		}

		/// <summary>
		/// Writes sensitive information to <see cref="Serilogger.Information(string)"/>
		/// </summary>
		protected override void LogSensitiveString(string message)
		{
			Serilogger.Information(message);
		}

		/// <summary>
		/// Writes an informational message to <see cref="Serilogger.Information(string)"/>
		/// </summary>
		protected override void LogInfoString(string message)
		{
			Serilogger.Information(message);
		}

		/// <summary>
		/// Writes logging progress information such as "Process X is 24% done" to <see cref="Serilogger.Information(string)"/>
		/// </summary>
		protected override void LogProgressString(string message)
		{
			Serilogger.Information(message);
		}

		/// <summary>
		/// Writes a debugging message to <see cref="Serilogger.Verbose(string)"/>
		/// </summary>
		protected override void LogDebugString(string message)
		{
			Serilogger.Verbose(message);
		}

		/// <summary>
		/// Writes a warning message to <see cref="Serilogger.Warning(string)"/>
		/// </summary>
		protected override void LogWarningString(string message)
		{
			Serilogger.Warning(message);
		}

		/// <summary>
		/// Writes an error message to <see cref="Serilogger.Error(string)"/>
		/// </summary>
		protected override void LogErrorString(string message)
		{
			Serilogger.Error(message);
		}

		/// <summary>
		/// Writes a fatal error message to <see cref="Serilogger.Fatal(string)"/>
		/// </summary>
		protected override void LogFatalErrorString(string message)
		{
			Serilogger.Fatal(message);
		}
	}
}