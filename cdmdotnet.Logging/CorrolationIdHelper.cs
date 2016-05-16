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
	[Obsolete("Use CorrelationIdHelper")]
	public class CorrolationIdHelper : CorrelationIdHelper, ICorrolationIdHelper
	{
		#region Implementation of ICorrolationIdHelper

		[Obsolete("Use GetCorrelationId")]
		public Guid GetCorrolationId()
		{
			return GetCorrelationId();
		}

		[Obsolete("Use SetCorrolationId")]
		public Guid SetCorrolationId(Guid corrolationId)
		{
			return SetCorrelationId(corrolationId);
		}

		#endregion
	}
}