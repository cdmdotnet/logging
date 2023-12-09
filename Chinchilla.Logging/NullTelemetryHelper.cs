using System;
using System.Collections.Generic;

namespace Chinchilla.Logging
{
	/// <summary>
	/// Does nothing but stops <see cref="NullReferenceException"/> being raised.
	/// </summary>
	public class NullTelemetryHelper : ITelemetryHelper
	{
		#region Implementation of ITelemetryHelper

		/// <summary>
		/// Send an event.
		/// </summary>
		/// <param name="eventName">A name for the event.</param>
		/// <param name="properties">Named string values you can use to search and classify events.</param>
		public virtual void TrackEvent(string eventName, IDictionary<string, string> properties = null)
		{
		}

		/// <summary>
		/// Send a metric.
		/// </summary>
		/// <param name="name">Metric name.</param>
		/// <param name="value">Metric value.</param>
		/// <param name="properties">Named string values you can use to search and classify events.</param>
		public virtual void TrackMetric(string name, double value, IDictionary<string, string> properties = null)
		{
		}

		/// <summary>
		/// Send an Exception
		/// </summary>
		/// <param name="exception">The exception to log.</param>
		/// <param name="metrics">Additional values associated with this exception.</param>
		/// <param name="properties">Named string values you can use to search and classify events.</param>
		public virtual void TrackException(Exception exception, IDictionary<string, double> metrics = null, IDictionary<string, string> properties = null)
		{
		}

		/// <summary>
		/// Send information about an external dependency call in the application, such as a call to a database.
		/// </summary>
		/// <param name="dependencyName">External dependency name.</param>
		/// <param name="commandName">Dependency call command name.</param>
		/// <param name="startTime">The time when the dependency was called.</param>
		/// <param name="duration">The time taken by the external dependency to handle the call.</param>
		/// <param name="wasSuccessfull">True if the dependency call was handled successfully.</param>
		/// <param name="properties">Named string values you can use to search and classify events.</param>
		public virtual void TrackDependency(string dependencyName, string commandName, DateTimeOffset startTime, TimeSpan duration, bool wasSuccessfull, IDictionary<string, string> properties = null)
		{
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
		/// <param name="properties">Named string values you can use to search and classify events.</param>
		public virtual void TrackDependency(string dependencyTypeName, string target, string dependencyName, string data, DateTimeOffset startTime, TimeSpan duration, string resultCode, bool wasSuccessfull, IDictionary<string, string> properties = null)
		{
		}

		/// <summary>
		/// Send information about a request handled by the application.
		/// </summary>
		/// <param name="name">The request name.</param>
		/// <param name="startTime">The time when the page was requested.</param>
		/// <param name="duration">The time taken by the application to handle the request.</param>
		/// <param name="responseCode">The response status code.</param>
		/// <param name="wasSuccessfull">True if the request was handled successfully by the application.</param>
		/// <param name="properties">Named string values you can use to search and classify events.</param>
		public virtual void TrackRequest(string name, DateTimeOffset startTime, TimeSpan duration, string responseCode, bool wasSuccessfull, IDictionary<string, string> properties = null)
		{
		}

		/// <summary>
		/// Send information about a request handled by the application.
		/// </summary>
		/// <param name="name">The request name.</param>
		/// <param name="url"></param>
		/// <param name="userId">The ID of user accessing the application.</param>
		/// <param name="startTime">The time when the page was requested.</param>
		/// <param name="duration">The time taken by the application to handle the request.</param>
		/// <param name="responseCode">The response status code.</param>
		/// <param name="wasSuccessfull">True if the request was handled successfully by the application.</param>
		/// <param name="properties">Named string values you can use to search and classify events.</param>
		/// <param name="sessionId">The application-defined session ID.</param>
		public void TrackRequest(string name, Uri url, string userId, DateTimeOffset startTime, TimeSpan duration, string responseCode, bool wasSuccessfull, IDictionary<string, string> properties = null, string sessionId = null)
		{
		}

		/// <summary>
		/// Send a trace message for display in Diagnostic Search.
		/// </summary>
		/// <param name="message">Message to display.</param>
		/// <param name="severityLevel">
		/// Trace severity level.
		/// 
		/// Verbose = 0,
		/// Information = 1,
		/// Warning = 2,
		/// Error = 3,
		/// Critical = 4
		/// </param>
		/// <param name="properties">Named string values you can use to search and classify events.</param>
		/// <param name="sessionId">The application-defined session ID</param>
		public void TrackTrace(string message, int severityLevel = 1, IDictionary<string, string> properties = null, string sessionId = null)
		{
		}

		/// <summary>
		/// Send a trace message for display in Diagnostic Search.
		/// </summary>
		/// <param name="message">Message to display.</param>
		/// <param name="severityLevel">
		/// Trace severity level.
		/// 
		/// Verbose = 0,
		/// Information = 1,
		/// Warning = 2,
		/// Error = 3,
		/// Critical = 4
		/// </param>
		/// <param name="userId">The ID of user accessing the application.</param>
		/// <param name="properties">Named string values you can use to search and classify events.</param>
		/// <param name="sessionId">The application-defined session ID</param>
		public void TrackTrace(string message, string userId, int severityLevel = 1, IDictionary<string, string> properties = null, string sessionId = null)
		{
		}

		/// <summary>
		/// Send information about a process flow in the application.
		/// </summary>
		/// <param name="name">Name of the process</param>
		/// <param name="processFlowStepType">Type of process step within the process flow</param>
		/// <param name="processFlowStepName">Name of the step within the process flow</param>
		/// <param name="processFlowHost">Host that is processing this process step within the process flow</param>
		/// <param name="sequence">The value that defines absolute order of this step within large process flows.</param>
		/// <param name="duration">The time taken by the application to handle the request.</param>
		/// <param name="responseCode">The response status code.</param>
		/// <param name="wasSuccessfull">True if the request was handled successfully by the application.</param>
		/// <param name="properties">Named string values you can use to search and classify events.</param>
		public void TrackProcessFlow(string name, string processFlowStepType, string processFlowStepName, string processFlowHost, TimeSpan duration, string responseCode, bool wasSuccessfull, string sequence = null, IDictionary<string, string> properties = null)
		{
		}

		/// <summary>
		/// Send information about a process flow in the application.
		/// </summary>
		/// <param name="name">Name of the process</param>
		/// <param name="url"></param>
		/// <param name="processFlowStepType">Type of process step within the process flow</param>
		/// <param name="processFlowStepName">Name of the step within the process flow</param>
		/// <param name="processFlowHost">Host that is processing this process step within the process flow</param>
		/// <param name="userId">The ID of user accessing the application.</param>
		/// <param name="sequence">The value that defines absolute order of this step within large process flows.</param>
		/// <param name="duration">The time taken by the application to handle the request.</param>
		/// <param name="responseCode">The response status code.</param>
		/// <param name="wasSuccessfull">True if the request was handled successfully by the application.</param>
		/// <param name="properties">Named string values you can use to search and classify events.</param>
		/// <param name="sessionId">The application-defined session ID.</param>
		public void TrackProcessFlow(string name, Uri url, string processFlowStepType, string processFlowStepName, string processFlowHost, string userId, TimeSpan duration, string responseCode, bool wasSuccessfull, string sequence = null, IDictionary<string, string> properties = null, string sessionId = null)
		{
		}

		/// <summary>
		/// Flushes the in-memory buffer, if one exists
		/// </summary>
		public virtual void Flush()
		{
		}

		#endregion
	}
}