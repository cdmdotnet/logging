#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="cdmdotnet Limited">
// // 	Copyright cdmdotnet Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using cdmdotnet.Logging.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace cdmdotnet.Logging
{
	/// <summary>
	/// Provides a set of methods that help you log events relating to the execution of your code.
	/// </summary>
	public abstract class PrimitiveLogger : VeryPrimitiveLogger
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="StringLogger"/> class.
		/// </summary>
		protected PrimitiveLogger(ILoggerSettings loggerSettings, ICorrelationIdHelper correlationIdHelper)
			:base(loggerSettings, correlationIdHelper)
		{
		}

		/// <summary>
		/// Format a message based on the input parameters.
		/// </summary>
		protected virtual string GenerateLogMessage(string level, string message, string container, Exception exception, IDictionary<string, object> additionalData, IDictionary<string, object> metaData)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(container))
				{
					var stackTrace = new StackTrace();
					StackFrame[] stackFrames = stackTrace.GetFrames();
					if (stackFrames != null)
					{
						foreach (StackFrame frame in stackFrames)
						{
							MethodBase method = frame.GetMethod();
							if (method.ReflectedType == null)
								continue;

							try
							{
								bool found = false;
								if (ExclusionNamespaces.Any(@namespace => !method.ReflectedType.FullName.StartsWith(@namespace)))
								{
									container = string.Format("{0}.{1}", method.ReflectedType.FullName, method.Name);
									found = true;
								}
								if (found)
									break;
							}
							catch
							{
								// Just move on
							}
						}
					}
				}
			}
			catch
			{
				// Just move on
			}


			Guid corrolationId = Guid.Empty;
			try
			{
				corrolationId = CorrelationIdHelper.GetCorrelationId();
			}
			catch
			{
				// Default already set
			}


			string pattern = "[{0}] {1:r}:";
			if (corrolationId != Guid.Empty)
				pattern = "[{0}] [{7:N}] {1:r}:";
			if (!string.IsNullOrWhiteSpace(container))
				pattern = string.Concat(pattern, " {3}:: {2}");
			if (exception != null)
				pattern = string.Concat(pattern, "\r\n{4}");
			if (additionalData != null)
				pattern = string.Concat(pattern, "\r\n{8}");
			if (metaData != null)
				pattern = string.Concat(pattern, "\r\n{9}");
			string messageToLog = string.Format(pattern, level, // 0
				DateTime.Now, // 1
				message, // 2
				container, // 3
				exception, // 4
				exception == null ? null : exception.Message, // 5
				exception == null ? null : exception.StackTrace, // 6
				corrolationId, // 7
				JsonConvert.SerializeObject(additionalData), // 8
				JsonConvert.SerializeObject(metaData) // 9
			);

			return messageToLog;
		}
	}
}