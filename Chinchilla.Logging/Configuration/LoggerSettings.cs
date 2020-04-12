#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="Chinchilla Software Limited">
// // 	Copyright Chinchilla Software Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System.Configuration;

namespace Chinchilla.Logging.Configuration
{
	/// <summary>
	/// The settings for all <see cref="ILogger"/> instances reading settings <see cref="LoggerSettingsConfigurationSection"/>.
	/// </summary>
	public class LoggerSettings : ConfigurationSection, ILoggerSettings
	{
		#region Implementation of ILoggerSettings

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableSensitive"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		public bool EnableSensitive
		{
			get { return LoggerSettingsConfigurationSection.Current.EnableSensitive; }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableInfo"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		public bool EnableInfo
		{
			get { return LoggerSettingsConfigurationSection.Current.EnableInfo; }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableProgress"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		public bool EnableProgress
		{
			get { return LoggerSettingsConfigurationSection.Current.EnableProgress; }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableDebug"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		public bool EnableDebug
		{
			get { return LoggerSettingsConfigurationSection.Current.EnableDebug; }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableWarning"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		public bool EnableWarning
		{
			get { return LoggerSettingsConfigurationSection.Current.EnableWarning; }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableError"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		public bool EnableError
		{
			get { return LoggerSettingsConfigurationSection.Current.EnableError; }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableFatalError"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		public bool EnableFatalError
		{
			get { return LoggerSettingsConfigurationSection.Current.EnableFatalError; }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableThreadedLogging"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		public bool EnableThreadedLogging
		{
			get { return LoggerSettingsConfigurationSection.Current.EnableThreadedLogging; }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.ModuleName"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		public string ModuleName
		{
			get { return LoggerSettingsConfigurationSection.Current.ModuleName; }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.Instance"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		public string Instance
		{
			get { return LoggerSettingsConfigurationSection.Current.Instance; }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnvironmentInstance"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		public string EnvironmentInstance
		{
			get { return LoggerSettingsConfigurationSection.Current.EnvironmentInstance; }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.Environment"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		public string Environment
		{
			get { return LoggerSettingsConfigurationSection.Current.Environment; }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.SqlDatabaseLogsConnectionStringName"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		public string SqlDatabaseLogsConnectionStringName
		{
			get { return LoggerSettingsConfigurationSection.Current.SqlDatabaseLogsConnectionStringName; }
		}

		/// <summary>
		/// The name of the table to use.
		/// </summary>
		public string SqlDatabaseTableName
		{
			get { return LoggerSettingsConfigurationSection.Current.SqlDatabaseTableName; }
		}

		/// <summary>
		/// If true, all log calls will be telemetered.
		/// </summary>
		public bool UseApplicationInsightTelemetryHelper
		{
			get { return LoggerSettingsConfigurationSection.Current.UseApplicationInsightTelemetryHelper; }
		}

		/// <summary>
		/// If true, all log calls will be telemetered.
		/// </summary>
		public bool UsePerformanceCounters
		{
			get { return LoggerSettingsConfigurationSection.Current.UsePerformanceCounters; }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableThreadedLoggingOutput"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		public bool EnableThreadedLoggingOutput
		{
			get { return LoggerSettingsConfigurationSection.Current.EnableThreadedLoggingOutput; }
		}

		#endregion

		string ILoggerSettings.GetConnectionString(string connectionStringName)
		{
			return GetConnectionString(connectionStringName);
		}

		/// <summary>
		/// Gets a connection string
		/// </summary>
		protected virtual string GetConnectionString(string connectionStringName)
		{
			ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringName];
			if (connectionStringSettings == null)
				throw new ConfigurationErrorsException(string.Format("No connection string named '{0}' was provided", connectionStringName));
			string connectionString = connectionStringSettings.ConnectionString;
			if (string.IsNullOrWhiteSpace(connectionStringName))
				throw new ConfigurationErrorsException(string.Format("No value for the connection string named '{0}' was provided", connectionStringName));
			return connectionString;
		}
	}
}