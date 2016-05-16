#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="cdmdotnet Limited">
// // 	Copyright cdmdotnet Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System;

namespace cdmdotnet.Logging
{
	public interface ILogger
	{
		void LogInfo(string message, string container = null, Exception exception = null);

		void LogDebug(string message, string container = null, Exception exception = null);

		void LogWarning(string message, string container = null, Exception exception = null);

		void LogError(string message, string container = null, Exception exception = null);

		void LogFatalError(string message, string container = null, Exception exception = null);
	}
}