#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="cdmdotnet Limited">
// // 	Copyright cdmdotnet Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System.Configuration;
using cdmdotnet.Logging.Configuration;
using Microsoft.Azure;

namespace cdmdotnet.Logging.Azure.Configuration
{
	/// <summary>
	/// The settings for all <see cref="ILogger"/> instances reading settings from the Azure Portal.
	/// </summary>
	public class AzureLoggerSettingsConfiguration
		: ILoggerSettings
		, IContainerLoggerSettings
	{
		#region Implementation of ILoggerSettings

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableSensitive"/> from <see cref="AppSettingsSection"/> of the app.config or web.config file with Azure portal providing runtime overrides.
		/// </summary>
		public bool EnableSensitive
		{
			get { return ((IContainerLoggerSettings)this).EnableSensitive(null); }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableInfo"/> from <see cref="AppSettingsSection"/> of the app.config or web.config file with Azure portal providing runtime overrides.
		/// </summary>
		public bool EnableInfo
		{
			get { return ((IContainerLoggerSettings)this).EnableInfo(null); }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableProgress"/> from <see cref="AppSettingsSection"/> of the app.config or web.config file with Azure portal providing runtime overrides.
		/// </summary>
		public bool EnableProgress
		{
			get { return ((IContainerLoggerSettings)this).EnableProgress(null); }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableDebug"/> from <see cref="AppSettingsSection"/> of the app.config or web.config file with Azure portal providing runtime overrides.
		/// </summary>
		public bool EnableDebug
		{
			get { return ((IContainerLoggerSettings)this).EnableDebug(null); }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableWarning"/> from <see cref="AppSettingsSection"/> of the app.config or web.config file with Azure portal providing runtime overrides.
		/// </summary>
		public bool EnableWarning
		{
			get { return ((IContainerLoggerSettings)this).EnableWarning(null); }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableError"/> from <see cref="AppSettingsSection"/> of the app.config or web.config file with Azure portal providing runtime overrides.
		/// </summary>
		public bool EnableError
		{
			get { return ((IContainerLoggerSettings)this).EnableError(null); }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableFatalError"/> from <see cref="AppSettingsSection"/> of the app.config or web.config file with Azure portal providing runtime overrides.
		/// </summary>
		public bool EnableFatalError
		{
			get { return ((IContainerLoggerSettings)this).EnableFatalError(null); }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableThreadedLogging"/> from <see cref="AppSettingsSection"/> of the app.config or web.config file with Azure portal providing runtime overrides.
		/// </summary>
		public bool EnableThreadedLogging
		{
			get { return ((IContainerLoggerSettings)this).EnableThreadedLogging(null); }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableThreadedLoggingOutput"/> from <see cref="AppSettingsSection"/> of the app.config or web.config file with Azure portal providing runtime overrides.
		/// </summary>
		public bool EnableThreadedLoggingOutput
		{
			get { return bool.Parse(CloudConfigurationManager.GetSetting("EnableThreadedLoggingOutput", false) ?? "false"); }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.ModuleName"/> from <see cref="AppSettingsSection"/> of the app.config or web.config file with Azure portal providing runtime overrides.
		/// </summary>
		public string ModuleName
		{
			get { return CloudConfigurationManager.GetSetting("ModuleName", false) ?? "All"; }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.Instance"/> from <see cref="AppSettingsSection"/> of the app.config or web.config file with Azure portal providing runtime overrides.
		/// </summary>
		public string Instance
		{
			get { return CloudConfigurationManager.GetSetting("Instance", false) ?? "All"; }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnvironmentInstance"/> from <see cref="AppSettingsSection"/> of the app.config or web.config file with Azure portal providing runtime overrides.
		/// </summary>
		public string EnvironmentInstance
		{
			get { return CloudConfigurationManager.GetSetting("EnvironmentInstance", false) ?? "All"; }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.Environment"/> from <see cref="AppSettingsSection"/> of the app.config or web.config file with Azure portal providing runtime overrides.
		/// </summary>
		public string Environment
		{
			get { return CloudConfigurationManager.GetSetting("Environment", false) ?? "Unknown"; }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.SqlDatabaseLogsConnectionStringName"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		public string SqlDatabaseLogsConnectionStringName
		{
			get { return ((IContainerLoggerSettings)this).SqlDatabaseLogsConnectionStringName(null); }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.SqlDatabaseTableName"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		public string SqlDatabaseTableName
		{
			get { return ((IContainerLoggerSettings)this).SqlDatabaseTableName(null); }
		}

		/// <summary>
		/// If true, all log calls will be telemetered.
		/// </summary>
		public bool UseApplicationInsightTelemetryHelper
		{
			get { return ((IContainerLoggerSettings)this).UseApplicationInsightTelemetryHelper(null); }
		}

		/// <summary>
		/// If true, all log calls will be telemetered.
		/// </summary>
		public bool UsePerformanceCounters
		{
			get { return ((IContainerLoggerSettings)this).UsePerformanceCounters(null); }
		}

		#endregion

		#region Implementation of IContainerLoggerSettings

		/// <summary>
		/// If false <see cref="ILogger.LogSensitive"/> will not do anything nor log anything.
		/// </summary>
		bool IContainerLoggerSettings.EnableSensitive(string container)
		{
			return GetBooleanValue("EnableLogSensitive", container, "false", "EnableSensitive");
		}

		/// <summary>
		/// If false <see cref="ILogger.LogInfo"/> will not do anything nor log anything.
		/// </summary>
		bool IContainerLoggerSettings.EnableInfo(string container)
		{
			return GetBooleanValue("EnableLogInfo", container, "true", "EnableInfo");
		}

		/// <summary>
		/// If false <see cref="ILogger.LogProgress"/> will not do anything nor log anything.
		/// </summary>
		bool IContainerLoggerSettings.EnableProgress(string container)
		{
			return GetBooleanValue("EnableLogProgress", container, "true", "EnableProgress");
		}

		/// <summary>
		/// If false <see cref="ILogger.LogDebug"/> will not do anything nor log anything.
		/// </summary>
		bool IContainerLoggerSettings.EnableDebug(string container)
		{
			return GetBooleanValue("EnableLogDebug", container, "false", "EnableDebug");
		}

		/// <summary>
		/// If false <see cref="ILogger.LogWarning"/> will not do anything nor log anything.
		/// </summary>
		bool IContainerLoggerSettings.EnableWarning(string container)
		{
			return GetBooleanValue("EnableLogWarning", container, "true", "EnableWarning");
		}

		/// <summary>
		/// If false <see cref="ILogger.LogError"/> will not do anything nor log anything.
		/// </summary>
		bool IContainerLoggerSettings.EnableError(string container)
		{
			return GetBooleanValue("EnableLogError", container, "true", "EnableError");
		}

		/// <summary>
		/// If false <see cref="ILogger.LogFatalError"/> will not do anything nor log anything.
		/// </summary>
		bool IContainerLoggerSettings.EnableFatalError(string container)
		{
			return GetBooleanValue("EnableLogFatalError", container, "true", "EnableFatalError");
		}

		/// <summary>
		/// If true, the <see cref="ILogger"/> will use one extra thread per instance to persist logs. 
		/// This means the log methods like <see cref="ILogger.LogInfo"/> will return indicating the information is in a queue to be logged.
		/// This greatly increases the performance in the event your <see cref="ILogger"/> is under heavy load, for example, if your logging database is under strain, your application will continue to perform, but logs will be queued.
		/// </summary>
		bool IContainerLoggerSettings.EnableThreadedLogging(string container)
		{
			return GetBooleanValue("EnableThreadedLoggingOutput", container, "false");
		}

		/// <summary>
		/// The key of the <see cref="ConfigurationManager.AppSettings"/> item that holds the name of the connection string to use.
		/// </summary>
		string IContainerLoggerSettings.SqlDatabaseLogsConnectionStringName(string container)
		{
			return GetStringValue("SqlDatabaseLogsConnectionStringName", container, "Logs");
		}

		/// <summary>
		/// The name of the table to use.
		/// </summary>
		string IContainerLoggerSettings.SqlDatabaseTableName(string container)
		{
			return GetStringValue("SqlDatabaseTableName", container, "Logs");
		}

		/// <summary>
		/// If true, all log calls will be telemetered.
		/// </summary>
		bool IContainerLoggerSettings.UseApplicationInsightTelemetryHelper(string container)
		{
			return GetBooleanValue("UseApplicationInsightTelemetryHelper", container, "false");
		}

		/// <summary>
		/// If true, all log calls will be telemetered.
		/// </summary>
		bool IContainerLoggerSettings.UsePerformanceCounters(string container)
		{
			return GetBooleanValue("UsePerformanceCounters", container, "false");
		}

		#endregion

		/// <summary />
		protected virtual bool GetBooleanValue(string key, string container, string defaultValue, string key2 = null)
		{
			string value = null;
			if (!string.IsNullOrWhiteSpace(container))
			{
				value = CloudConfigurationManager.GetSetting(string.Format("{0}.{1}", key, container), false);
				if (string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(key2))
					value = CloudConfigurationManager.GetSetting(string.Format("{0}.{1}", key2, container), false);
			}
			if (string.IsNullOrWhiteSpace(value))
			{
				value = CloudConfigurationManager.GetSetting(key, false);
				if (string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(key2))
					value = CloudConfigurationManager.GetSetting(key2, false);
			}
			return bool.Parse(value ?? defaultValue);
		}

		/// <summary />
		protected virtual string GetStringValue(string key, string container, string defaultValue, string key2 = null)
		{
			string value = null;
			if (!string.IsNullOrWhiteSpace(container))
			{
				value = CloudConfigurationManager.GetSetting(string.Format("{0}.{1}", key, container), false);
				if (string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(key2))
					value = CloudConfigurationManager.GetSetting(string.Format("{0}.{1}", key2, container), false);
			}
			if (string.IsNullOrWhiteSpace(value))
			{
				value = CloudConfigurationManager.GetSetting(key, false);
				if (string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(key2))
					value = CloudConfigurationManager.GetSetting(key2, false);
			}
			return value ?? defaultValue;
		}
	}
}