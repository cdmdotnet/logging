#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="cdmdotnet Limited">
// // 	Copyright cdmdotnet Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System;
using System.Threading;
using System.Web;
using cdmdotnet.StateManagement;
using cdmdotnet.StateManagement.Threaded;

namespace cdmdotnet.Logging
{
	/// <summary>
	/// A <see cref="ICorrelationIdHelper"/> that uses the <see cref="ThreadedContextItemCollectionFactory.GetCurrentContext"/> method to get a transport collection within the <see cref="Thread"/>.
	/// </summary>
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
		public CorrelationIdHelper(ThreadedContextItemCollectionFactory contextItemCollectionFactory)
		{
			ContextItemCollection = contextItemCollectionFactory.GetCurrentContext();
		}

		private const string CallContextPermissionScopeValueKey = "CorrelationIdValue";

		/// <summary>
		/// The local collection that holds values.
		/// </summary>
		protected IContextItemCollection ContextItemCollection { get; private set; }

		#region Implementation of ICorrelationIdHelper

		/// <summary>
		/// Get the CorrelationId set via <see cref="SetCorrelationId"/> 
		/// first using <see cref="HttpContext.Items"/> if <see cref="HttpContext.Current"/> is not null
		/// if <see cref="HttpContext.Current"/> is null OR there is a <see cref="NullReferenceException"/> then
		/// try from, within this <see cref="Thread"/>.
		/// </summary>
		public Guid GetCorrelationId()
		{
			return ContextItemCollection.GetData<Guid>(CallContextPermissionScopeValueKey);
		}

		/// <summary>
		/// Set the CorrelationId within this <see cref="Thread"/> that can be read via <see cref="GetCorrelationId"/>.
		/// If <see cref="HttpContext.Current"/> is not null then also try and set it using <see cref="HttpContext.Items"/>.
		/// </summary>
		public Guid SetCorrelationId(Guid correlationId)
		{
			return ContextItemCollection.SetData(CallContextPermissionScopeValueKey, correlationId);
		}

		#endregion
	}
}