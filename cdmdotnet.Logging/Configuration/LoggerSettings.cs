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
		/// Reads the <see cref="ILoggerSettings.EnableThreadedLoggingOutput"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		public bool EnableThreadedLoggingOutput
		{
			get { return LoggerSettingsConfigurationSection.Current.EnableThreadedLoggingOutput; }
		}

		#endregion
	}
}