#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="cdmdotnet Limited">
// // 	Copyright cdmdotnet Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System.Configuration;
using Chinchilla.Logging.Configuration;

#if NETSTANDARD2_0
using Microsoft.Extensions.Configuration;
#else
using Microsoft.Azure;
#endif

namespace Chinchilla.Logging.Azure.Configuration
{
	/// <summary>
	/// The settings for all <see cref="ILogger"/> instances reading settings from the Azure Portal.
	/// </summary>
	public class AzureLoggerSettingsConfiguration
		: ILoggerSettings
		, IContainerLoggerSettings
	{
#if NETSTANDARD2_0
		/// <summary>
		/// Instantiates a new instance of the <see cref="AzureLoggerSettingsConfiguration"/> class using the provided <paramref name="configuration"/> to get configuration settings from.
		/// </summary>
		public AzureLoggerSettingsConfiguration(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		/// <summary>
		/// The <see cref="IConfiguration"/> to get application settings from.
		/// </summary>
		protected IConfiguration Configuration { get; }
#endif

		#region Implementation of ILoggerSettings

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableSensitive"/> from app setting of the app.config, web.config or .Net Core equivalent file with Azure portal providing runtime overrides.
		/// </summary>
		public bool EnableSensitive
		{
			get { return ((IContainerLoggerSettings)this).EnableSensitive(null); }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableInfo"/> from app setting of the app.config, web.config or .Net Core equivalent file with Azure portal providing runtime overrides.
		/// </summary>
		public bool EnableInfo
		{
			get { return ((IContainerLoggerSettings)this).EnableInfo(null); }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableProgress"/> from app setting of the app.config, web.config or .Net Core equivalent file with Azure portal providing runtime overrides.
		/// </summary>
		public bool EnableProgress
		{
			get { return ((IContainerLoggerSettings)this).EnableProgress(null); }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableDebug"/> from app setting of the app.config, web.config or .Net Core equivalent file with Azure portal providing runtime overrides.
		/// </summary>
		public bool EnableDebug
		{
			get { return ((IContainerLoggerSettings)this).EnableDebug(null); }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableWarning"/> from app setting of the app.config, web.config or .Net Core equivalent file with Azure portal providing runtime overrides.
		/// </summary>
		public bool EnableWarning
		{
			get { return ((IContainerLoggerSettings)this).EnableWarning(null); }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableError"/> from app setting of the app.config, web.config or .Net Core equivalent file with Azure portal providing runtime overrides.
		/// </summary>
		public bool EnableError
		{
			get { return ((IContainerLoggerSettings)this).EnableError(null); }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableFatalError"/> from app setting of the app.config, web.config or .Net Core equivalent file with Azure portal providing runtime overrides.
		/// </summary>
		public bool EnableFatalError
		{
			get { return ((IContainerLoggerSettings)this).EnableFatalError(null); }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableThreadedLogging"/> from app setting of the app.config, web.config or .Net Core equivalent file with Azure portal providing runtime overrides.
		/// </summary>
		public bool EnableThreadedLogging
		{
			get { return ((IContainerLoggerSettings)this).EnableThreadedLogging(null); }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableThreadedLoggingOutput"/> from app setting of the app.config, web.config or .Net Core equivalent file with Azure portal providing runtime overrides.
		/// </summary>
		public bool EnableThreadedLoggingOutput
		{
			get { return bool.Parse(GetSetting("EnableThreadedLoggingOutput") ?? "false"); }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.ModuleName"/> from app setting of the app.config, web.config or .Net Core equivalent file with Azure portal providing runtime overrides.
		/// </summary>
		public string ModuleName
		{
			get { return GetSetting("ModuleName") ?? "All"; }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.Instance"/> from app setting of the app.config, web.config or .Net Core equivalent file with Azure portal providing runtime overrides.
		/// </summary>
		public string Instance
		{
			get { return GetSetting("Instance") ?? "All"; }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnvironmentInstance"/> from app setting of the app.config, web.config or .Net Core equivalent file with Azure portal providing runtime overrides.
		/// </summary>
		public string EnvironmentInstance
		{
			get { return GetSetting("EnvironmentInstance") ?? "All"; }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.Environment"/> from app setting of the app.config, web.config or .Net Core equivalent file with Azure portal providing runtime overrides.
		/// </summary>
		public string Environment
		{
			get { return GetSetting("Environment") ?? "Unknown"; }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.SqlDatabaseLogsConnectionStringName"/> from the app setting of the app.config, web.config or .Net Core equivalent file with Azure portal providing runtime overrides.
		/// </summary>
		public string SqlDatabaseLogsConnectionStringName
		{
			get { return ((IContainerLoggerSettings)this).SqlDatabaseLogsConnectionStringName(null); }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.SqlDatabaseTableName"/> from the app setting of the app.config, web.config or .Net Core equivalent file with Azure portal providing runtime overrides.
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
		/// The key of the app setting item that holds the name of the connection string to use.
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

		/// <summary>
		/// Reads configurations settings from .NET Core and .NET Framework support app.config, web.config and other locations, including Azure Portal.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		protected virtual string GetSetting(string name)
		{
#if NETSTANDARD2_0
			return Configuration.GetValue<string>($"Chinchilla.Logging.{name}".Replace(".", ":"));
#else
			return CloudConfigurationManager.GetSetting(name, false);
#endif
		}

		/// <summary />
		protected virtual bool GetBooleanValue(string key, string container, string defaultValue, string key2 = null)
		{
			string value = GetStringValue(key, container, defaultValue, key2);
			return bool.Parse(value);
		}

		/// <summary />
		protected virtual string GetStringValue(string key, string container, string defaultValue, string key2 = null)
		{
			string value = null;
#if NETSTANDARD2_0
			if (!string.IsNullOrWhiteSpace(container))
			{
				try
				{
					value = GetSetting($"{container}.{key}");
				}
				catch { }
				if (string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(key2))
				{
					try
					{
						value = GetSetting($"{container}.{key2}");
					}
					catch { }
				}
			}
			if (string.IsNullOrWhiteSpace(value))
			{
				try
				{
					value = GetSetting(key);
				}
				catch { }
				if (string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(key2))
				{
					try
					{
						value = GetSetting(key2);
					}
					catch { }
				}
			}
#else
			if (!string.IsNullOrWhiteSpace(container))
			{
				value = GetSetting(string.Format("{0}.{1}", key, container));
				if (string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(key2))
					value = GetSetting(string.Format("{0}.{1}", key2, container));
			}
			if (string.IsNullOrWhiteSpace(value))
			{
				value = GetSetting(key);
				if (string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(key2))
					value = GetSetting(key2);
			}
#endif
			return value ?? defaultValue;
		}
	}
}