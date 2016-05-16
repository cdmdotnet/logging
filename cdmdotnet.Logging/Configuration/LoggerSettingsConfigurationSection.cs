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
	/// The settings for all <see cref="ILogger"/> instances reading information from an app.config or web.config file.
	/// </summary>
	/// <remarks>
	/// http://haacked.com/archive/2007/03/12/custom-configuration-sections-in-3-easy-steps.aspx
	/// </remarks>
	public class LoggerSettingsConfigurationSection : ConfigurationSection, ILoggerSettings
	{
		private const string ConfigurationSectionKey = "LoggerSettings";

		private static readonly LoggerSettingsConfigurationSection Settings = ConfigurationManager.GetSection(ConfigurationSectionKey) as LoggerSettingsConfigurationSection;

		/// <summary />
		public static ILoggerSettings Default
		{
			get
			{
				return Settings;
			}
		}

		#region Implementation of ILoggerSettings

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

		#endregion
	}
}