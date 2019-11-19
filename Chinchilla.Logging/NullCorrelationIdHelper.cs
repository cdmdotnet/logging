#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="Chinchilla Software Limited">
// // 	Copyright Chinchilla Software Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System;

namespace Chinchilla.Logging
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