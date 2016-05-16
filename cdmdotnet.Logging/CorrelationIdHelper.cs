#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="cdmdotnet Limited">
// // 	Copyright cdmdotnet Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace cdmdotnet.Logging
{
	/// <summary>
	/// A <see cref="ICorrelationIdHelper"/> that uses the <see cref="CallContext"/> to transporting the CorrelationId within the <see cref="Thread"/>
	/// </summary>
	public class CorrelationIdHelper : ICorrelationIdHelper
	{
		private const string CallContextPermissoinScopeValueKey = "CorrelationIdValue";

		#region Implementation of ICorrolationIdHelper

		/// <summary>
		/// Get the CorrelationId set via <see cref="SetCorrelationId"/> within this <see cref="Thread"/> via <see cref="CallContext.GetData"/>.
		/// </summary>
		public Guid GetCorrelationId()
		{
			return (Guid)CallContext.GetData(CallContextPermissoinScopeValueKey);
		}

		/// <summary>
		/// Set the CorrelationId within this <see cref="Thread"/> via <see cref="CallContext.SetData"/> that can be read via <see cref="GetCorrelationId"/>.
		/// </summary>
		public Guid SetCorrelationId(Guid corrolationId)
		{
			CallContext.SetData(CallContextPermissoinScopeValueKey, corrolationId);
			return corrolationId;
		}

		#endregion
	}
}