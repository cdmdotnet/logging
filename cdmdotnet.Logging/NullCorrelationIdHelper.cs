#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="cdmdotnet Limited">
// // 	Copyright cdmdotnet Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System;

namespace cdmdotnet.Logging
{
	/// <summary>
	/// Always returns <see cref="Guid.Empty"/>
	/// </summary>
	public class NullCorrelationIdHelper : ICorrelationIdHelper
	{
		/// <summary>
		/// Does nothing
		/// </summary>
		/// <returns><see cref="Guid.Empty"/></returns>
		public Guid GetCorrelationId()
		{
			return Guid.Empty;
		}

		/// <summary>
		/// Returns <see cref="Guid.Empty"/>
		/// </summary>
		/// <returns><see cref="Guid.Empty"/></returns>
		public Guid SetCorrelationId(Guid correlationId)
		{
			return Guid.Empty;
		}
	}
}