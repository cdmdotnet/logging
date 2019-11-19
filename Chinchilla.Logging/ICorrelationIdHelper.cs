#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="Chinchilla Software Limited">
// // 	Copyright Chinchilla Software Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System;
using System.Threading;

namespace Chinchilla.Logging
{
	/// <summary>
	/// A CorrelationId is an identifier used to group together logs. This helper manages passing the value within the <see cref="Thread"/> without having to manually set it on ever call to the <see cref="ILogger"/>.
	/// </summary>
	/// <remarks>https://dzimchuk.net/event-correlation-in-application-insights/</remarks>
	public interface ICorrelationIdHelper
	{
		/// <summary>
		/// Get the CorrelationId set via <see cref="SetCorrelationId"/>.
		/// </summary>
		Guid GetCorrelationId();

		/// <summary>
		/// Set the CorrelationId.
		/// </summary>
		Guid SetCorrelationId(Guid correlationId);
	}
}