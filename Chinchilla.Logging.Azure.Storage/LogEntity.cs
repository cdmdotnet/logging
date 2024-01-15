#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="Chinchilla Software Limited">
// // 	Copyright Chinchilla Software Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;

namespace Chinchilla.Logging.Azure.Storage
{
	/// <summary>
	/// Information about an event to be logged
	/// </summary>
	public class LogEntity : ITableEntity
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
			((ITableEntity)this).PartitionKey = level;
			((ITableEntity)this).RowKey = uniqueId;
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

		private DateTime raised { get; set; }

		/// <summary>
		/// The <see cref="DateTime"/> the event was raised.
		/// </summary>
		public DateTime Raised
		{
			get
			{
				return raised;
			}
			set
			{
				raised = value;
				((ITableEntity)this).Timestamp = value.ToUniversalTime();
			}
		}

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

		/// <summary>
		/// A friendly identifier to help identify different applications if they use the same <see cref="ILogger"/>.
		/// </summary>
		public string Module { get; set; }

		/// <summary>
		/// A friendly identifier to help identify different instances of the same application, such as a development or production instance of the same application.
		/// </summary>
		public string Instance { get; set; }

		/// <summary />
		public string Environment { get; set; }

		/// <summary>
		/// A friendly identifier to help identify different environments of the same application, such as deployments to different geo-graphical locations of the same application.
		/// </summary>
		public string EnvironmentInstance { get; set; }

		string ITableEntity.PartitionKey { get; set; }

		string ITableEntity.RowKey { get; set; }

		DateTimeOffset? ITableEntity.Timestamp { get; set; }

		ETag ITableEntity.ETag { get; set; }
	}
}