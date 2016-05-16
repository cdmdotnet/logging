#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="cdmdotnet Limited">
// // 	Copyright cdmdotnet Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace cdmdotnet.Logging.Configuration
{
	public interface ILoggerSettings
	{
		bool EnableInfo { get; }

		bool EnableDebug { get; }

		bool EnableWarning { get; }

		bool EnableError { get; }

		bool EnableFatalError { get; }

		bool EnableThreadedLogging { get; }

		bool EnableThreadedLoggingOutput { get; }

		string ModuleName { get; }

		string Instance { get; }

		string EnvironmentInstance { get; }

		string Environment { get; }
	}
}