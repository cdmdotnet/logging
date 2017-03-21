#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="cdmdotnet Limited">
// // 	Copyright cdmdotnet Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System.Threading;
using cdmdotnet.StateManagement.Web;

namespace cdmdotnet.Logging
{
	/// <summary>
	/// A <see cref="ICorrelationIdHelper"/> that uses the <see cref="WebContextItemCollectionFactory.GetCurrentContext"/> method to get a transport collection within the <see cref="Thread"/>.
	/// </summary>
	/// <remarks>https://dzimchuk.net/event-correlation-in-application-insights/</remarks>
	public class WebCorrelationIdHelper : CorrelationIdHelper
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="WebCorrelationIdHelper"/> class requiring <paramref name="contextItemCollectionFactory"/>, used to set the internal collection with an instance.
		/// </summary>
		public WebCorrelationIdHelper(WebContextItemCollectionFactory contextItemCollectionFactory)
			: base(contextItemCollectionFactory.GetCurrentContext())
		{
		}
	}
}