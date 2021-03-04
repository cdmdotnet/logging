using System;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos.Table;

namespace Chinchilla.Logging.Azure.Storage
{
	/// <summary>
	/// Information about an event to be logged
	/// </summary>
	public class LogEntity : TableEntity
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="LogEntity"/>.
		/// </summary>
		public LogEntity(string level)
			: this(level, Guid.NewGuid().ToString("N"))
		{
		}

		/// <summary>
		/// Instantiates a new instance of the <see cref="LogEntity"/>.
		/// </summary>
		public LogEntity(string level, string uniqueId)
		{
			PartitionKey = level;
			RowKey = uniqueId;
		}

		/// <summary>
		/// Instantiates a new instance of the <see cref="LogEntity"/>.
		/// </summary>
		public LogEntity(LogInformation logInformation)
			: this (logInformation.Level)
		{
			Raised = logInformation.Raised;
			Level = logInformation.Level;
			Message = logInformation.Message;
			Container = logInformation.Container;
			Exception = logInformation.Exception;
			MetaData = logInformation.MetaData;
			CorrolationId = logInformation.CorrolationId;
		}

		/// <summary>
		/// The <see cref="DateTime"/> the event was raised.
		/// </summary>
		public DateTime Raised { get; set; }

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
		public string Exception { get; set; }

		/// <summary>
		/// A serialised <see cref="IDictionary{TKey,TValue}"/> if one was provided.
		/// </summary>
		public string MetaData { get; set; }

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