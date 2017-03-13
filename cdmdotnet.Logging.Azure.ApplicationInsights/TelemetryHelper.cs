using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace cdmdotnet.Logging.Azure.ApplicationInsights
{
	/// <summary>
	/// An <see cref="ITelemetryHelper"/> implemented with Application Insights.
	/// </summary>
	public class TelemetryHelper : ITelemetryHelper
	{
		/// <summary>
		/// Instantiate a new instance of <see cref="TelemetryHelper"/>
		/// </summary>
		public TelemetryHelper(TelemetryClient telemetryClient)
		{
			TelemetryClient = telemetryClient;
		}

		/// <summary>
		/// Instantiate a new instance of <see cref="TelemetryHelper"/>
		/// </summary>
		public TelemetryHelper()
			:this(new TelemetryClient())
		{
			TelemetryClient.InstrumentationKey = TelemetryConfiguration.Active.InstrumentationKey;
		}

		/// <summary>
		/// The <see cref="TelemetryClient"/> used to send telemetry data.
		/// </summary>
		protected TelemetryClient TelemetryClient { get; set; }

		#region Implementation of ITelemetryHelper

		/// <summary>
		/// Send an event.
		/// </summary>
		/// <param name="eventName">A name for the event.</param>
		/// <param name="properties">Named string values you can use to search and classify events.</param>
		public void TrackEvent(string eventName, IDictionary<string, string> properties = null)
		{
			TelemetryClient.TrackEvent(eventName, properties);
		}

		/// <summary>
		/// Send a metric.
		/// </summary>
		/// <param name="name">Metric name.</param>
		/// <param name="value">Metric value.</param>
		/// <param name="properties">Named string values you can use to search and classify events.</param>
		public void TrackMetric(string name, double value, IDictionary<string, string> properties = null)
		{
			TelemetryClient.TrackMetric(name, value, properties);
		}

		/// <summary>
		/// Send an Exception
		/// </summary>
		/// <param name="exception">The exception to log.</param>
		/// <param name="metrics">Additional values associated with this exception.</param>
		/// <param name="properties">Named string values you can use to search and classify events.</param>
		public void TrackException(Exception exception, IDictionary<string, double> metrics = null, IDictionary<string, string> properties = null)
		{
			TelemetryClient.TrackException(exception, properties, metrics);
		}

		/// <summary>
		/// Send information about an external dependency call in the application, such as a call to a database.
		/// </summary>
		/// <param name="dependencyName">External dependency name.</param>
		/// <param name="commandName">Dependency call command name.</param>
		/// <param name="startTime">The time when the dependency was called.</param>
		/// <param name="duration">The time taken by the external dependency to handle the call.</param>
		/// <param name="wasSuccessfull">True if the dependency call was handled successfully.</param>
		public void TrackDependency(string dependencyName, string commandName, DateTimeOffset startTime, TimeSpan duration, bool wasSuccessfull)
		{
			TelemetryClient.TrackDependency(dependencyName, commandName, startTime, duration, wasSuccessfull);
		}

		/// <summary>
		/// Send information about an external dependency call in the application, such as a web-service call
		/// </summary>
		/// <param name="dependencyTypeName">External dependency type.</param>
		/// <param name="target">External dependency target.</param>
		/// <param name="dependencyName">External dependency name.</param>
		/// <param name="data">Dependency call command name.</param>
		/// <param name="startTime">The time when the dependency was called.</param>
		/// <param name="duration">The time taken by the external dependency to handle the call.</param>
		/// <param name="resultCode">Result code of dependency call execution.</param>
		/// <param name="wasSuccessfull">True if the dependency call was handled successfully.</param>
		public void TrackDependency(string dependencyTypeName, string target, string dependencyName, string data, DateTimeOffset startTime, TimeSpan duration, string resultCode, bool wasSuccessfull)
		{
			TelemetryClient.TrackDependency(dependencyTypeName, target, dependencyName, data, startTime, duration, resultCode, wasSuccessfull);
		}

		/// <summary>
		/// Send information about a request handled by the application.
		/// </summary>
		/// <param name="name">The request name.</param>
		/// <param name="startTime">The time when the page was requested.</param>
		/// <param name="duration">The time taken by the application to handle the request.</param>
		/// <param name="responseCode">The response status code.</param>
		/// <param name="wasSuccessfull">True if the request was handled successfully by the application.</param>
		public void TrackRequest(string name, DateTimeOffset startTime, TimeSpan duration, string responseCode, bool wasSuccessfull)
		{
			TelemetryClient.TrackRequest(name, startTime, duration, responseCode, wasSuccessfull);
		}

		#endregion
	}
}
