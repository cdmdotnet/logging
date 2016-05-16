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
	[Obsolete("Use NullCorrelationIdHelper")]
	public class NullCorrolationIdHelper : NullCorrelationIdHelper, ICorrolationIdHelper
	{
		[Obsolete("Use GetCorrelationId")]
		public Guid GetCorrolationId()
		{
			return GetCorrelationId();
		}

		[Obsolete("Use SetCorrelationId")]
		public Guid SetCorrolationId(Guid corrolationId)
		{
			return SetCorrelationId(corrolationId);
		}
	}
}