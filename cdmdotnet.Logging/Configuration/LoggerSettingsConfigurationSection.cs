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
	/// <remarks>
	/// http://haacked.com/archive/2007/03/12/custom-configuration-sections-in-3-easy-steps.aspx
	/// </remarks>
	public class LoggerSettingsConfigurationSection : ConfigurationSection, ILoggerSettings
	{
		private const string ConfigurationSectionKey = "LoggerSettings";

		private static readonly LoggerSettingsConfigurationSection Settings = ConfigurationManager.GetSection(ConfigurationSectionKey) as LoggerSettingsConfigurationSection;

		public static ILoggerSettings Default
		{
			get
			{
				return Settings;
			}
		}

		#region Implementation of ILoggerSettings

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

		#endregion

		#region ILoggerSettings Members

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

		#endregion
	}
}