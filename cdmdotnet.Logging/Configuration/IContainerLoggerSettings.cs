#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="cdmdotnet Limited">
// // 	Copyright cdmdotnet Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System.Configuration;

namespace cdmdotnet.Logging.Configuration
{
	/// <summary>
	/// The settings for all <see cref="ILogger"/> instances.
	/// </summary>
	public interface IContainerLoggerSettings
	{
		/// <summary>
		/// If false <see cref="ILogger.LogSensitive"/> will not do anything nor log anything.
		/// </summary>
		bool EnableSensitive(string container);

		/// <summary>
		/// If false <see cref="ILogger.LogInfo"/> will not do anything nor log anything.
		/// </summary>
		bool EnableInfo(string container);

		/// <summary>
		/// If false <see cref="ILogger.LogProgress"/> will not do anything nor log anything.
		/// </summary>
		bool EnableProgress(string container);

		/// <summary>
		/// If false <see cref="ILogger.LogDebug"/> will not do anything nor log anything.
		/// </summary>
		bool EnableDebug(string container);

		/// <summary>
		/// If false <see cref="ILogger.LogWarning"/> will not do anything nor log anything.
		/// </summary>
		bool EnableWarning(string container);

		/// <summary>
		/// If false <see cref="ILogger.LogError"/> will not do anything nor log anything.
		/// </summary>
		bool EnableError(string container);

		/// <summary>
		/// If false <see cref="ILogger.LogFatalError"/> will not do anything nor log anything.
		/// </summary>
		bool EnableFatalError(string container);

		/// <summary>
		/// If true, the <see cref="ILogger"/> will use one extra thread per instance to persist logs. 
		/// This means the log methods like <see cref="ILogger.LogInfo"/> will return indicating the information is in a queue to be logged.
		/// This greatly increases the performance in the event your <see cref="ILogger"/> is under heavy load, for example, if your logging database is under strain, your application will continue to perform, but logs will be queued.
		/// </summary>
		bool EnableThreadedLogging(string container);

		/// <summary>
		/// The key of the <see cref="ConfigurationManager.AppSettings"/> item that holds the name of the connection string to use.
		/// </summary>
		string SqlDatabaseLogsConnectionStringName(string container);

		/// <summary>
		/// The name of the table to use.
		/// </summary>
		string SqlDatabaseTableName(string container);

		/// <summary>
		/// If true, all log calls will be telemetered.
		/// </summary>
		bool UseApplicationInsightTelemetryHelper(string container);

		/// <summary>
		/// If true, all log calls will be telemetered.
		/// </summary>
		bool UsePerformanceCounters(string container);
	}
}