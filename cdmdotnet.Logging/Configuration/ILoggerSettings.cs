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
	public interface ILoggerSettings
	{
		/// <summary>
		/// If false <see cref="ILogger.LogSensitive"/> will not do anything nor log anything.
		/// </summary>
		bool EnableSensitive { get; }

		/// <summary>
		/// If false <see cref="ILogger.LogInfo"/> will not do anything nor log anything.
		/// </summary>
		bool EnableInfo { get; }

		/// <summary>
		/// If false <see cref="ILogger.LogProgress"/> will not do anything nor log anything.
		/// </summary>
		bool EnableProgress { get; }

		/// <summary>
		/// If false <see cref="ILogger.LogDebug"/> will not do anything nor log anything.
		/// </summary>
		bool EnableDebug { get; }

		/// <summary>
		/// If false <see cref="ILogger.LogWarning"/> will not do anything nor log anything.
		/// </summary>
		bool EnableWarning { get; }

		/// <summary>
		/// If false <see cref="ILogger.LogError"/> will not do anything nor log anything.
		/// </summary>
		bool EnableError { get; }

		/// <summary>
		/// If false <see cref="ILogger.LogFatalError"/> will not do anything nor log anything.
		/// </summary>
		bool EnableFatalError { get; }

		/// <summary>
		/// If true, the <see cref="ILogger"/> will use one extra thread per instance to persist logs. 
		/// This means the log methods like <see cref="ILogger.LogInfo"/> will return indicating the information is in a queue to be logged.
		/// This greatly increases the performance in the event your <see cref="ILogger"/> is under heavy load, for example, if your logging database is under strain, your application will continue to perform, but logs will be queued.
		/// </summary>
		bool EnableThreadedLogging { get; }

		/// <summary>
		/// Doesn't do anything just yet
		/// </summary>
		bool EnableThreadedLoggingOutput { get; }

		/// <summary>
		/// A friendly identifier to help identify different applications if they use the same <see cref="ILogger"/>.
		/// </summary>
		string ModuleName { get; }

		/// <summary>
		/// A friendly identifier to help identify different instances of the same application, such as a development or production instance of the same application.
		/// </summary>
		string Instance { get; }

		/// <summary>
		/// A friendly identifier to help identify different environments of the same application, such as deployments to different geo-graphical locations of the same application.
		/// </summary>
		string EnvironmentInstance { get; }

		/// <summary />
		string Environment { get; }

		/// <summary>
		/// The key of the <see cref="ConfigurationManager.AppSettings"/> item that holds the name of the connection string to use.
		/// </summary>
		string SqlDatabaseLogsConnectionStringName { get; }

		/// <summary>
		/// The name of the table to use.
		/// </summary>
		string SqlDatabaseTableName { get; }
	}
}