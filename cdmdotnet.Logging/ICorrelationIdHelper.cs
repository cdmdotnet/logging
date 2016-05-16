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
	public interface ICorrelationIdHelper
	{
		Guid GetCorrelationId();

		Guid SetCorrelationId(Guid correlationId);
	}
}