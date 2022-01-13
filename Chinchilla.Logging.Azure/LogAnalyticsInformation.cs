#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="Chinchilla Software Limited">
// // 	Copyright Chinchilla Software Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;

using Chinchilla.Logging.Configuration;
using Newtonsoft.Json;

namespace Chinchilla.Logging.Azure
{
	/// <summary>
	/// Information about an event to be logged
	/// </summary>
	public class LogAnalyticsInformation
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="LogInformation"/> class with <see cref="Raised"/> set to a default value of <see cref="DateTime.UtcNow"/>
		/// </summary>
		public LogAnalyticsInformation(ILoggerSettings loggerSettings, LogInformation logInformation)
		{
			Raised = logInformation.Raised;
			Level = logInformation.Level;
			Message = logInformation.Message;
			Container = logInformation.Container;
			Exception = JsonConvert.DeserializeObject<Exception>(logInformation.Exception, Logger.DefaultJsonSerializerSettings);
			AdditionalData = logInformation.AdditionalData;
			MetaData = JsonConvert.DeserializeObject<IDictionary<string, object>>(logInformation.MetaData, Logger.DefaultJsonSerializerSettings);
			CorrolationId = logInformation.CorrolationId;

			Module = loggerSettings.ModuleName;
			Instance = loggerSettings.Instance;
			Environment = loggerSettings.Environment;
			EnvironmentInstance = loggerSettings.EnvironmentInstance;
		}

		/// <summary>
		/// The <see cref="DateTime"/> the event was raised.
		/// </summary>
		public DateTime Raised { get; private set; }

		/// <summary>
		/// The level of the event, such as 'error', 'info' or 'debug'
		/// </summary>
		public string Level { get; set; }

		/// <summary>
		/// A Human readable message describing the event
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// The container such as a method or class the event was raised from
		/// </summary>
		public string Container { get; set; }

		/// <summary>
		/// A serialised <see cref="Exception"/> if one was provided.
		/// </summary>
		public Exception Exception { get; set; }

		/// <summary>
		/// An un-serialised <see cref="IDictionary{TKey,TValue}"/> if one was provided.
		/// </summary>
		public IDictionary<string, object> AdditionalData { get; set; }

		/// <summary>
		/// A serialised <see cref="IDictionary{TKey,TValue}"/> if one was provided.
		/// </summary>
		public IDictionary<string, object> MetaData { get; set; }

		/// <summary>
		/// The value from <see cref="ICorrelationIdHelper.GetCorrelationId"/>
		/// </summary>
		public Guid CorrolationId { get; set; }

		/// <summary />
		public string Module { get; set; }

		/// <summary />
		public string Instance { get; set; }

		/// <summary />
		public string Environment { get; set; }

		/// <summary />
		public string EnvironmentInstance { get; set; }
	}
}