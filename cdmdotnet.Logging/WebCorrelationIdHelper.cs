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
using System.Web;

namespace cdmdotnet.Logging
{
	/// <summary>
	/// A <see cref="ICorrelationIdHelper"/> that uses the <see cref="HttpContext.Items"/> to transporting the CorrelationId within the <see cref="Thread"/>
	/// </summary>
	public class WebCorrelationIdHelper : ICorrelationIdHelper
	{
		private const string CallContextPermissionScopeValueKey = "CorrelationIdValue";

		#region Implementation of ICorrelationIdHelper

		/// <summary>
		/// Get the CorrelationId set via <see cref="SetCorrelationId"/> 
		/// first using <see cref="HttpContext.Items"/> if <see cref="HttpContext.Current"/> is not null
		/// if <see cref="HttpContext.Current"/> is null OR there is a <see cref="NullReferenceException"/> then
		/// try from, within this <see cref="Thread"/>.
		/// </summary>
		public Guid GetCorrelationId()
		{
			try
			{
				if (HttpContext.Current != null)
					return (Guid)HttpContext.Current.Items[CallContextPermissionScopeValueKey];
			}
			catch (NullReferenceException)
			{
			}
			return (Guid)CallContext.GetData(CallContextPermissionScopeValueKey);
		}

		/// <summary>
		/// Set the CorrelationId within this <see cref="Thread"/> that can be read via <see cref="GetCorrelationId"/>.
		/// If <see cref="HttpContext.Current"/> is not null then also try and set it using <see cref="HttpContext.Items"/>.
		/// </summary>
		public Guid SetCorrelationId(Guid correlationId)
		{
			CallContext.SetData(CallContextPermissionScopeValueKey, correlationId);
			try
			{
				if (HttpContext.Current != null)
					HttpContext.Current.Items[CallContextPermissionScopeValueKey] = correlationId;
			}
			catch (NullReferenceException)
			{
			}
			return correlationId;
		}

		#endregion
	}
}