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

namespace cdmdotnet.Logging
{
	/// <summary>
	/// A <see cref="ICorrelationIdHelper"/> that uses the <see cref="HttpContext.Items"/> to transporting the CorrelationId within the <see cref="Thread"/>
	/// </summary>
	public class WebCorrelationIdHelper : ICorrelationIdHelper
	{
		private const string CallContextPermissoinScopeValueKey = "CorrelationIdValue";

		#region Implementation of ICorrolationIdHelper

		/// <summary>
		/// Get the CorrelationId set via <see cref="SetCorrelationId"/> within this <see cref="Thread"/> via <see cref="HttpContext.Items"/>.
		/// </summary>
		public Guid GetCorrelationId()
		{
			return (Guid)HttpContext.Current.Items[CallContextPermissoinScopeValueKey];
		}

		/// <summary>
		/// Set the CorrelationId within this <see cref="Thread"/> via <see cref="HttpContext.Items"/> that can be read via <see cref="GetCorrelationId"/>.
		/// </summary>
		public Guid SetCorrelationId(Guid corrolationId)
		{
			HttpContext.Current.Items[CallContextPermissoinScopeValueKey] = corrolationId;
			return corrolationId;
		}

		#endregion
	}
}