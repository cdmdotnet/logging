using System;
using System.Collections.Generic;

namespace cdmdotnet.Logging
{
	/// <summary>
	/// Information about an event to be logged
	/// </summary>
	public class LogInformation
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="LogInformation"/> class with <see cref="Raised"/> set to a default value of <see cref="DateTime.UtcNow"/>
		/// </summary>
		public LogInformation()
		{
			Raised = DateTime.UtcNow;
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
		public string Exception { get; set; }

		/// <summary>
		/// An un-serialised <see cref="IDictionary{TKey,TValue}"/> if one was provided.
		/// </summary>
		public IDictionary<string, object> AdditionalData { get; set; }

		/// <summary>
		/// A serialised <see cref="IDictionary{TKey,TValue}"/> if one was provided.
		/// </summary>
		public string MetaData { get; set; }

		/// <summary>
		/// The value from <see cref="ICorrelationIdHelper.GetCorrelationId"/>
		/// </summary>
		public Guid CorrolationId { get; set; }
	}
}