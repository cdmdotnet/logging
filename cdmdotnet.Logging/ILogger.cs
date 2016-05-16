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
	/// <summary>
	/// Provides a set of methods that help you log events relating to the execution of your code.
	/// </summary>
	public interface ILogger
	{
		/// <summary>
		/// Writes an informational message to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		void LogInfo(string message, string container = null, Exception exception = null);

		/// <summary>
		/// Writes a debugging message to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		void LogDebug(string message, string container = null, Exception exception = null);

		/// <summary>
		/// Writes a warning message to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		void LogWarning(string message, string container = null, Exception exception = null);

		/// <summary>
		/// Writes an error message to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		void LogError(string message, string container = null, Exception exception = null);

		/// <summary>
		/// Writes a fatal error message to the <see cref="ILogger"/> using the specified <paramref name="message"></paramref>.
		/// </summary>
		void LogFatalError(string message, string container = null, Exception exception = null);
	}
}