#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="cdmdotnet Limited">
// // 	Copyright cdmdotnet Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System;
using System.Runtime.Remoting.Messaging;

namespace cdmdotnet.Logging
{
	public class CorrelationIdHelper : ICorrelationIdHelper
	{
		private const string CallContextPermissoinScopeValueKey = "CorrelationIdValue";

		#region Implementation of ICorrolationIdHelper

		public Guid GetCorrelationId()
		{
			return (Guid)CallContext.GetData(CallContextPermissoinScopeValueKey);
		}

		public Guid SetCorrelationId(Guid corrolationId)
		{
			CallContext.SetData(CallContextPermissoinScopeValueKey, corrolationId);
			return corrolationId;
		}

		#endregion
	}
}