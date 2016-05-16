#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="cdmdotnet Limited">
// // 	Copyright cdmdotnet Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using cdmdotnet.Logging.Configuration;
using Microsoft.Azure;

namespace cdmdotnet.Logging.Azure.Configuration
{
	public class AzureLoggerSettingsConfiguration : ILoggerSettings
	{
		#region Implementation of ILoggerSettings

		public bool EnableInfo
		{
			get { return bool.Parse(CloudConfigurationManager.GetSetting("EnableInfo") ?? "true"); }
		}

		public bool EnableDebug
		{
			get { return bool.Parse(CloudConfigurationManager.GetSetting("EnableDebug") ?? "false"); }
		}

		public bool EnableWarning
		{
			get { return bool.Parse(CloudConfigurationManager.GetSetting("EnableWarning") ?? "true"); }
		}

		public bool EnableError
		{
			get { return bool.Parse(CloudConfigurationManager.GetSetting("EnableError") ?? "true"); }
		}

		public bool EnableFatalError
		{
			get { return bool.Parse(CloudConfigurationManager.GetSetting("EnableFatalError") ?? "true"); }
		}

		public bool EnableThreadedLogging
		{
			get { return bool.Parse(CloudConfigurationManager.GetSetting("EnableThreadedLogging") ?? "true"); }
		}

		public bool EnableThreadedLoggingOutput
		{
			get { return bool.Parse(CloudConfigurationManager.GetSetting("EnableThreadedLoggingOutput") ?? "false"); }
		}

		public string ModuleName
		{
			get { return CloudConfigurationManager.GetSetting("ModuleName") ?? "All"; }
		}

		public string Instance
		{
			get { return CloudConfigurationManager.GetSetting("Instance") ?? "All"; }
		}

		public string EnvironmentInstance
		{
			get { return CloudConfigurationManager.GetSetting("EnvironmentInstance") ?? "All"; }
		}

		public string Environment
		{
			get { return CloudConfigurationManager.GetSetting("Environment") ?? "Unknown"; }
		}

		#endregion
	}
}
