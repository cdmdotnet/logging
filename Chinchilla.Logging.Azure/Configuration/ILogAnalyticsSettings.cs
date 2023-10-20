#if NET40
#else
using Chinchilla.Logging.Configuration;

namespace Chinchilla.Logging.Azure.Configuration
{
	/// <summary>
	/// The settings for <see cref="LogAnalyticsLogger"/> instances.
	/// </summary>
	public interface ILogAnalyticsSettings
		: ILoggerSettings
	{
		/// <summary>
		/// The WorkspaceID is the unique identifier for the Log Analytics workspace.
		/// See https://docs.microsoft.com/en-us/azure/azure-monitor/logs/data-collector-api#authorization.
		/// </summary>
		string WorkspaceId { get; }

		/// <summary>
		/// The primary or the secondary key for the workspace to sign the request with.
		/// See https://docs.microsoft.com/en-us/azure/azure-monitor/logs/data-collector-api#authorization.
		/// </summary>
		string SharedKey { get; }

		/// <summary>
		/// The custom record type.
		/// See https://docs.microsoft.com/en-us/azure/azure-monitor/logs/data-collector-api#record-type-and-properties.
		/// </summary>
		string LogType { get; }

		/// <summary>
		/// The WorkspaceID is the unique identifier for the Log Analytics workspace.
		/// See https://docs.microsoft.com/en-us/azure/azure-monitor/logs/data-collector-api#authorization.
		/// </summary>
		string GetWorkspaceId(string container);

		/// <summary>
		/// The primary or the secondary key for the workspace to sign the request with.
		/// See https://docs.microsoft.com/en-us/azure/azure-monitor/logs/data-collector-api#authorization.
		/// </summary>
		string GetSharedKey(string container);

		/// <summary>
		/// The custom record type.
		/// See https://docs.microsoft.com/en-us/azure/azure-monitor/logs/data-collector-api#record-type-and-properties.
		/// </summary>
		string GetLogType(string container);
	}
}
#endif