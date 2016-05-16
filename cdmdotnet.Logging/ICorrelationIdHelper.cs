#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="cdmdotnet Limited">
// // 	Copyright cdmdotnet Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System;
using System.Threading;

namespace cdmdotnet.Logging
{
	/// <summary>
	/// A CorrelationId is an identifier used to group together logs. This helper manages passing the value within the <see cref="Thread"/> without having to manually set it on ever call to the <see cref="ILogger"/>.
	/// </summary>
	public interface ICorrelationIdHelper
	{
		/// <summary>
		/// Get the CorrelationId set via <see cref="SetCorrelationId"/> within this <see cref="Thread"/>.
		/// </summary>
		Guid GetCorrelationId();

		/// <summary>
		/// Set the CorrelationId within this <see cref="Thread"/> that can be read via <see cref="GetCorrelationId"/>.
		/// </summary>
		Guid SetCorrelationId(Guid correlationId);
	}
}