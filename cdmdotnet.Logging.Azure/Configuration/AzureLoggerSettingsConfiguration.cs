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
	public class AzureLoggerSettingsConfiguration : ILoggerSettings
	{
		#region Implementation of ILoggerSettings

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableInfo"/> from <see cref="AppSettingsSection"/> of the app.config or web.config file with Azure portal providing runtime overrides.
		/// </summary>
		public bool EnableInfo
		{
			get { return bool.Parse(CloudConfigurationManager.GetSetting("EnableInfo") ?? "true"); }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableDebug"/> from <see cref="AppSettingsSection"/> of the app.config or web.config file with Azure portal providing runtime overrides.
		/// </summary>
		public bool EnableDebug
		{
			get { return bool.Parse(CloudConfigurationManager.GetSetting("EnableDebug") ?? "false"); }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableWarning"/> from <see cref="AppSettingsSection"/> of the app.config or web.config file with Azure portal providing runtime overrides.
		/// </summary>
		public bool EnableWarning
		{
			get { return bool.Parse(CloudConfigurationManager.GetSetting("EnableWarning") ?? "true"); }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableError"/> from <see cref="AppSettingsSection"/> of the app.config or web.config file with Azure portal providing runtime overrides.
		/// </summary>
		public bool EnableError
		{
			get { return bool.Parse(CloudConfigurationManager.GetSetting("EnableError") ?? "true"); }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableFatalError"/> from <see cref="AppSettingsSection"/> of the app.config or web.config file with Azure portal providing runtime overrides.
		/// </summary>
		public bool EnableFatalError
		{
			get { return bool.Parse(CloudConfigurationManager.GetSetting("EnableFatalError") ?? "true"); }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableThreadedLogging"/> from <see cref="AppSettingsSection"/> of the app.config or web.config file with Azure portal providing runtime overrides.
		/// </summary>
		public bool EnableThreadedLogging
		{
			get { return bool.Parse(CloudConfigurationManager.GetSetting("EnableThreadedLogging") ?? "true"); }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnableThreadedLoggingOutput"/> from <see cref="AppSettingsSection"/> of the app.config or web.config file with Azure portal providing runtime overrides.
		/// </summary>
		public bool EnableThreadedLoggingOutput
		{
			get { return bool.Parse(CloudConfigurationManager.GetSetting("EnableThreadedLoggingOutput") ?? "false"); }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.ModuleName"/> from <see cref="AppSettingsSection"/> of the app.config or web.config file with Azure portal providing runtime overrides.
		/// </summary>
		public string ModuleName
		{
			get { return CloudConfigurationManager.GetSetting("ModuleName") ?? "All"; }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.Instance"/> from <see cref="AppSettingsSection"/> of the app.config or web.config file with Azure portal providing runtime overrides.
		/// </summary>
		public string Instance
		{
			get { return CloudConfigurationManager.GetSetting("Instance") ?? "All"; }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.EnvironmentInstance"/> from <see cref="AppSettingsSection"/> of the app.config or web.config file with Azure portal providing runtime overrides.
		/// </summary>
		public string EnvironmentInstance
		{
			get { return CloudConfigurationManager.GetSetting("EnvironmentInstance") ?? "All"; }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.Environment"/> from <see cref="AppSettingsSection"/> of the app.config or web.config file with Azure portal providing runtime overrides.
		/// </summary>
		public string Environment
		{
			get { return CloudConfigurationManager.GetSetting("Environment") ?? "Unknown"; }
		}

		/// <summary>
		/// Reads the <see cref="ILoggerSettings.SqlDatabaseLogsConnectionStringName"/> from the <see cref="ConfigurationSection"/> in your app.config or web.config file.
		/// </summary>
		public string SqlDatabaseLogsConnectionStringName
		{
			get { return CloudConfigurationManager.GetSetting("SqlDatabaseLogsConnectionStringName") ?? "Logs"; }
		}


		#endregion
	}
}