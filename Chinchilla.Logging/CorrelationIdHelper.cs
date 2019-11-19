#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="Chinchilla Software Limited">
// // 	Copyright Chinchilla Software Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System;
using System.Threading;
using Chinchilla.StateManagement;
using Chinchilla.StateManagement.Threaded;

namespace Chinchilla.Logging
{
	/// <summary>
	/// A <see cref="ICorrelationIdHelper"/> that uses the <see cref="ContextItemCollectionFactory.GetCurrentContext"/> method to get a transport collection within the <see cref="Thread"/>.
	/// </summary>
	/// <remarks>https://dzimchuk.net/event-correlation-in-application-insights/</remarks>
	public class CorrelationIdHelper : ICorrelationIdHelper
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="CorrelationIdHelper"/> class using the provided <paramref name="contextItemCollection"/> to set as the value of <see cref="ContextItemCollection"/>.
		/// </summary>
		protected CorrelationIdHelper(IContextItemCollection contextItemCollection)
		{
			ContextItemCollection = contextItemCollection;
		}

		/// <summary>
		/// Instantiates a new instance of the <see cref="CorrelationIdHelper"/> class requiring <paramref name="contextItemCollectionFactory"/>, used to set <see cref="ContextItemCollection"/> with an instance.
		/// </summary>
		public CorrelationIdHelper(ContextItemCollectionFactory contextItemCollectionFactory)
		{
			ContextItemCollection = contextItemCollectionFactory.GetCurrentContext();
		}

		/// <summary>
		/// The key used in the internal collection to find the CorrelationId value by.
		/// </summary>
		public const string CallContextCorrelationIdValueKey = "CorrelationIdValue";

		/// <summary>
		/// The local collection that holds values.
		/// </summary>
		protected IContextItemCollection ContextItemCollection { get; private set; }

		#region Implementation of ICorrelationIdHelper

		/// <summary>
		/// Get the CorrelationId set via <see cref="SetCorrelationId"/>.
		/// </summary>
		public Guid GetCorrelationId()
		{
			return ContextItemCollection.GetData<Guid>(CallContextCorrelationIdValueKey);
		}

		/// <summary>
		/// Set the CorrelationId.
		/// </summary>
		public Guid SetCorrelationId(Guid correlationId)
		{
			return ContextItemCollection.SetData(CallContextCorrelationIdValueKey, correlationId);
		}

		#endregion
	}
}