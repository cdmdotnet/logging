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
	[Obsolete("Use ICorrelationIdHelper")]
	public interface ICorrolationIdHelper : ICorrelationIdHelper
	{
		[Obsolete("Use GetCorrelationId")]
		Guid GetCorrolationId();

		[Obsolete("Use SetCorrolationId")]
		Guid SetCorrolationId(Guid corrolationId);
	}
}