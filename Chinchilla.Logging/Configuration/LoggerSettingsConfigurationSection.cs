#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="Chinchilla Software Limited">
// // 	Copyright Chinchilla Software Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System;
using System.Configuration;

namespace Chinchilla.Logging.Configuration
{
	/// <summary>
	/// The settings for all <see cref="ILogger"/> instances reading settings from an app.config or web.config file.
	/// This is used internally. Use <see cref="LoggerSettings"/> directly unless you know what you're doing.
	/// </summary>
	/// <remarks>
	/// http://haacked.com/archive/2007/03/12/custom-configuration-sections-in-3-easy-steps.aspx
	/// </remarks>
	public class LoggerSettingsConfigurationSection : ConfigurationSection, ILoggerSettings
	{
		private const string ConfigurationSectionKey = "LoggerSettings";

		private static readonly LoggerSettingsConfigurationSection Settings = ConfigurationManager.GetSection(ConfigurationSectionKey) as LoggerSettingsConfigurationSection;

		/// <summary />
		public static ILoggerSettings Current
		{
			get
			{
				if (Settings == null)
					throw new NullReferenceException("No LoggerSettings section was found in your app.config or web.config file. Alternatively it might not have all required settings defined... make sure ALL settings of the LoggerSettings have been defined.");
				return Settings;
			}
		}

		#region Implementation of ILoggerSettings

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableSensitive"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		[ConfigurationProperty("EnableSensitive", DefaultValue = false, IsRequired = true)]
		public bool EnableSensitive
		{
			get
			{
				return (bool)this["EnableSensitive"];
			}
			set
			{
				this["EnableSensitive"] = value;
			}
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableInfo"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		[ConfigurationProperty("EnableInfo", DefaultValue = true, IsRequired = true)]
		public bool EnableInfo
		{
			get
			{
				return (bool)this["EnableInfo"];
			}
			set
			{
				this["EnableInfo"] = value;
			}
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableProgress"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		[ConfigurationProperty("EnableProgress", DefaultValue = true, IsRequired = true)]
		public bool EnableProgress
		{
			get
			{
				return (bool)this["EnableProgress"];
			}
			set
			{
				this["EnableProgress"] = value;
			}
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableDebug"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		[ConfigurationProperty("EnableDebug", DefaultValue = false, IsRequired = true)]
		public bool EnableDebug
		{
			get
			{
				return (bool)this["EnableDebug"];
			}
			set
			{
				this["EnableDebug"] = value;
			}
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableWarning"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		[ConfigurationProperty("EnableWarning", DefaultValue = true, IsRequired = true)]
		public bool EnableWarning
		{
			get
			{
				return (bool)this["EnableWarning"];
			}
			set
			{
				this["EnableWarning"] = value;
			}
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableError"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		[ConfigurationProperty("EnableError", DefaultValue = true, IsRequired = true)]
		public bool EnableError
		{
			get
			{
				return (bool)this["EnableError"];
			}
			set
			{
				this["EnableError"] = value;
			}
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableFatalError"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		[ConfigurationProperty("EnableFatalError", DefaultValue = true, IsRequired = true)]
		public bool EnableFatalError
		{
			get
			{
				return (bool)this["EnableFatalError"];
			}
			set
			{
				this["EnableFatalError"] = value;
			}
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableThreadedLogging"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		[ConfigurationProperty("EnableThreadedLogging", DefaultValue = true, IsRequired = true)]
		public bool EnableThreadedLogging
		{
			get
			{
				return (bool)this["EnableThreadedLogging"];
			}
			set
			{
				this["EnableThreadedLogging"] = value;
			}
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.ModuleName"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		[ConfigurationProperty("ModuleName", DefaultValue = "All", IsRequired = true)]
		public string ModuleName
		{
			get
			{
				return (string)this["ModuleName"];
			}
			set
			{
				this["ModuleName"] = value;
			}
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.Instance"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		[ConfigurationProperty("Instance", DefaultValue = "All", IsRequired = true)]
		public string Instance
		{
			get
			{
				return (string)this["Instance"];
			}
			set
			{
				this["Instance"] = value;
			}
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnvironmentInstance"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		[ConfigurationProperty("EnvironmentInstance", DefaultValue = "All", IsRequired = true)]
		public string EnvironmentInstance
		{
			get
			{
				return (string)this["EnvironmentInstance"];
			}
			set
			{
				this["EnvironmentInstance"] = value;
			}
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.Environment"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		[ConfigurationProperty("Environment", DefaultValue = "Unknown", IsRequired = true)]
		public string Environment
		{
			get
			{
				return (string)this["Environment"];
			}
			set
			{
				this["Environment"] = value;
			}
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.SqlDatabaseLogsConnectionStringName"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		[ConfigurationProperty("SqlDatabaseLogsConnectionStringName", DefaultValue = "Logs", IsRequired = true)]
		public string SqlDatabaseLogsConnectionStringName
		{
			get
			{
				return (string)this["SqlDatabaseLogsConnectionStringName"];
			}
			set
			{
				this["SqlDatabaseLogsConnectionStringName"] = value;
			}
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.SqlDatabaseTableName"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		[ConfigurationProperty("SqlDatabaseTableName", DefaultValue = "Logs", IsRequired = true)]
		public string SqlDatabaseTableName
		{
			get
			{
				return (string)this["SqlDatabaseTableName"];
			}
			set
			{
				this["SqlDatabaseTableName"] = value;
			}
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.LogsConnectionStringName"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		[ConfigurationProperty("LogsConnectionStringName", DefaultValue = "Logs", IsRequired = true)]
		public string LogsConnectionStringName
		{
			get
			{
				return (string)this["LogsConnectionStringName"];
			}
			set
			{
				this["LogsConnectionStringName"] = value;
			}
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.LogsTableName"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		[ConfigurationProperty("LogsTableName", DefaultValue = "Logs", IsRequired = true)]
		public string LogsTableName
		{
			get
			{
				return (string)this["LogsTableName"];
			}
			set
			{
				this["LogsTableName"] = value;
			}
		}

		/// <summary>
		/// If true, all log calls will be telemetered.
		/// </summary>
		[ConfigurationProperty("UseApplicationInsightTelemetryHelper", DefaultValue = false, IsRequired = true)]
		public bool UseApplicationInsightTelemetryHelper
		{
			get
			{
				return (bool)this["UseApplicationInsightTelemetryHelper"];
			}
			set
			{
				this["UseApplicationInsightTelemetryHelper"] = value;
			}
		}

		/// <summary>
		/// If true, all log calls will be telemetered.
		/// </summary>
		[ConfigurationProperty("UsePerformanceCounters", DefaultValue = false, IsRequired = true)]
		public bool UsePerformanceCounters
		{
			get
			{
				return (bool)this["UsePerformanceCounters"];
			}
			set
			{
				this["UsePerformanceCounters"] = value;
			}
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableThreadedLoggingOutput"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		[ConfigurationProperty("EnableThreadedLoggingOutput", DefaultValue = false, IsRequired = true)]
		public bool EnableThreadedLoggingOutput
		{
			get
			{
				return (bool)this["EnableThreadedLoggingOutput"];
			}
			set
			{
				this["EnableThreadedLoggingOutput"] = value;
			}
		}

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

		#endregion
	}
}