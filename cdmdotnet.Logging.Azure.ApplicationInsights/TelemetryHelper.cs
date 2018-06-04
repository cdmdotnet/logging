using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using cdmdotnet.Logging.Configuration;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace cdmdotnet.Logging.Azure.ApplicationInsights
{
	/// <summary>
	/// An <see cref="ITelemetryHelper"/> implemented with Application Insights.
	/// </summary>
	public class TelemetryHelper : ITelemetryHelper
	{
		/// <summary>
		/// Indicates if all operations are handled off thread.
		/// </summary>
		public bool EnableThreadedOperations { get; set; }

		private ConcurrentQueue<Action> ActionQueue { get; set; }

		/// <summary>
		/// Generalised Logger settings.
		/// </summary>
		protected ILoggerSettings LoggerSettings { get; private set; }

		/// <summary>
		/// Instantiate a new instance of <see cref="TelemetryHelper"/>
		/// </summary>
		public TelemetryHelper(TelemetryClient telemetryClient, ICorrelationIdHelper correlationIdHelper, ILoggerSettings loggerSettings, bool enableThreadedOperations)
		{
			TelemetryClient = telemetryClient;
			CorrelationIdHelper = correlationIdHelper;
			ActionQueue = new ConcurrentQueue<Action>();
			EnableThreadedOperations = enableThreadedOperations;
			LoggerSettings = loggerSettings;

			if (EnableThreadedOperations)
			{
				Task.Factory.StartNew
				(
					() =>
					{
						long loop = long.MinValue;
						Action action;
						while (true)
						{
							if (ActionQueue.TryDequeue(out action))
								action();

							if (loop++ % 5 == 0)
								Thread.Yield();
							else
								Thread.Sleep(10);
							if (loop == long.MaxValue)
								loop = long.MinValue;
						}
					}
				);
			}
		}

		/// <summary>
		/// Instantiate a new instance of <see cref="TelemetryHelper"/>
		/// </summary>
		public TelemetryHelper(TelemetryClient telemetryClient, ICorrelationIdHelper correlationIdHelper)
			: this(telemetryClient, correlationIdHelper, null, false)
		{
		}

		/// <summary>
		/// Instantiate a new instance of <see cref="TelemetryHelper"/>
		/// </summary>
		public TelemetryHelper(ICorrelationIdHelper correlationIdHelper)
			: this(correlationIdHelper, false)
		{
		}

		/// <summary>
		/// Instantiate a new instance of <see cref="TelemetryHelper"/>
		/// </summary>
		public TelemetryHelper(ICorrelationIdHelper correlationIdHelper, bool enableThreadedOperations)
			: this(new TelemetryClient(), correlationIdHelper, null, enableThreadedOperations)
		{
			TelemetryClient.InstrumentationKey = TelemetryConfiguration.Active.InstrumentationKey;
		}

		/// <summary>
		/// Instantiate a new instance of <see cref="TelemetryHelper"/>
		/// </summary>
		public TelemetryHelper(ICorrelationIdHelper correlationIdHelper, ILoggerSettings loggerSettings)
			: this(correlationIdHelper, loggerSettings, false)
		{
		}

		/// <summary>
		/// Instantiate a new instance of <see cref="TelemetryHelper"/>
		/// </summary>
		public TelemetryHelper(ICorrelationIdHelper correlationIdHelper, ILoggerSettings loggerSettings, bool enableThreadedOperations)
			: this(new TelemetryClient(), correlationIdHelper, loggerSettings, enableThreadedOperations)
		{
			TelemetryClient.InstrumentationKey = TelemetryConfiguration.Active.InstrumentationKey;
		}

		/// <summary>
		/// The <see cref="TelemetryClient"/> used to send telemetry data.
		/// </summary>
		protected TelemetryClient TelemetryClient { get; set; }

		/// <summary>
		/// Obtains the CorrelationId to stamp each telemetry operation with
		/// </summary>
		protected ICorrelationIdHelper CorrelationIdHelper { get; private set; }

		#region Implementation of ITelemetryHelper

		/// <summary>
		/// Send an event.
		/// </summary>
		/// <param name="eventName">A name for the event.</param>
		/// <param name="properties">Named string values you can use to search and classify events.</param>
		public virtual void TrackEvent(string eventName, IDictionary<string, string> properties = null)
		{
			IDictionary<string, string> telemetryProperties = (properties ?? new Dictionary<string, string>());
			string correlationId = SetCorrelationId(telemetryProperties);

			if (LoggerSettings != null)
			{
				TelemetryClient.Context.Cloud.RoleName = LoggerSettings.ModuleName;
				TelemetryClient.Context.Cloud.RoleInstance = LoggerSettings.Instance;
			}

			if (EnableThreadedOperations)
				ActionQueue.Enqueue(() =>
				{
					TelemetryClient.Context.Operation.Id = correlationId;
					TelemetryClient.TrackEvent(eventName, telemetryProperties);
				});
			else
				TelemetryClient.TrackEvent(eventName, telemetryProperties);
		}

		/// <summary>
		/// Send a metric.
		/// </summary>
		/// <param name="name">Metric name.</param>
		/// <param name="value">Metric value.</param>
		/// <param name="properties">Named string values you can use to search and classify events.</param>
		public virtual void TrackMetric(string name, double value, IDictionary<string, string> properties = null)
		{
			IDictionary<string, string> telemetryProperties = (properties ?? new Dictionary<string, string>());
			string correlationId = SetCorrelationId(telemetryProperties);

			if (LoggerSettings != null)
			{
				TelemetryClient.Context.Cloud.RoleName = LoggerSettings.ModuleName;
				TelemetryClient.Context.Cloud.RoleInstance = LoggerSettings.Instance;
			}

			if (EnableThreadedOperations)
				ActionQueue.Enqueue(() =>
				{
					TelemetryClient.Context.Operation.Id = correlationId;
					TelemetryClient.TrackMetric(name, value, telemetryProperties);
				});
			else
				TelemetryClient.TrackMetric(name, value, telemetryProperties);
		}

		/// <summary>
		/// Send an Exception
		/// </summary>
		/// <param name="exception">The exception to log.</param>
		/// <param name="metrics">Additional values associated with this exception.</param>
		/// <param name="properties">Named string values you can use to search and classify events.</param>
		public virtual void TrackException(Exception exception, IDictionary<string, double> metrics = null, IDictionary<string, string> properties = null)
		{
			IDictionary<string, string> telemetryProperties = (properties ?? new Dictionary<string, string>());
			string correlationId = SetCorrelationId(telemetryProperties);

			if (LoggerSettings != null)
			{
				TelemetryClient.Context.Cloud.RoleName = LoggerSettings.ModuleName;
				TelemetryClient.Context.Cloud.RoleInstance = LoggerSettings.Instance;
			}

			if (EnableThreadedOperations)
				ActionQueue.Enqueue(() =>
				{
					TelemetryClient.Context.Operation.Id = correlationId;
					ActionQueue.Enqueue(() => TelemetryClient.TrackException(exception, telemetryProperties, metrics));
				});
			else
				TelemetryClient.TrackException(exception, telemetryProperties, metrics);
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
#pragma warning disable 618
			var dependencyTelemetry = new DependencyTelemetry(dependencyName, commandName, startTime, duration, wasSuccessfull);
#pragma warning restore 618
			if (properties != null)
				foreach (KeyValuePair<string, string> pair in properties)
					dependencyTelemetry.Properties.Add(pair);
			string correlationId = SetCorrelationId(dependencyTelemetry.Properties);

			TelemetryClient.Context.Operation.Name = dependencyName;
			dependencyTelemetry.Context.Operation.Id = correlationId;
			dependencyTelemetry.Context.Operation.Name = dependencyName;
			if (LoggerSettings != null)
			{
				dependencyTelemetry.Context.Cloud.RoleName = LoggerSettings.ModuleName;
				dependencyTelemetry.Context.Cloud.RoleInstance = LoggerSettings.Instance;
			}

			if (EnableThreadedOperations)
				ActionQueue.Enqueue(() =>
				{
					TelemetryClient.Context.Operation.Id = correlationId;
					ActionQueue.Enqueue(() => TelemetryClient.TrackDependency(dependencyName, commandName, startTime, duration, wasSuccessfull));
				});
			else
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
		/// <param name="properties">Named string values you can use to search and classify events.</param>
		public virtual void TrackDependency(string dependencyTypeName, string target, string dependencyName, string data, DateTimeOffset startTime, TimeSpan duration, string resultCode, bool wasSuccessfull, IDictionary<string, string> properties = null)
		{
			var dependencyTelemetry = new DependencyTelemetry(dependencyTypeName, target, dependencyName, data, startTime, duration, resultCode, wasSuccessfull);
			if (properties != null)
				foreach (KeyValuePair<string, string> pair in properties)
					dependencyTelemetry.Properties.Add(pair);
			string correlationId = SetCorrelationId(dependencyTelemetry.Properties);

			TelemetryClient.Context.Operation.Name = dependencyName;
			dependencyTelemetry.Context.Operation.Id = correlationId;
			dependencyTelemetry.Context.Operation.Name = dependencyName;
			if (LoggerSettings != null)
			{
				dependencyTelemetry.Context.Cloud.RoleName = LoggerSettings.ModuleName;
				dependencyTelemetry.Context.Cloud.RoleInstance = LoggerSettings.Instance;
			}

			if (EnableThreadedOperations)
				ActionQueue.Enqueue(() =>
				{
					TelemetryClient.Context.Operation.Id = correlationId;
					TelemetryClient.Context.Operation.Name = dependencyName;
					ActionQueue.Enqueue(() => TelemetryClient.TrackDependency(dependencyTelemetry));
				});
			else
				TelemetryClient.TrackDependency(dependencyTelemetry);
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
			TrackRequest(name, new Uri(string.Format("//{0}", name)), null, startTime, duration, responseCode, wasSuccessfull, properties);
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
		public virtual void TrackRequest(string name, Uri url, string userId, DateTimeOffset startTime, TimeSpan duration, string responseCode, bool wasSuccessfull, IDictionary<string, string> properties = null, string sessionId = null)
		{
			var requestTelemetry = new RequestTelemetry(name, startTime, duration, responseCode, wasSuccessfull)
			{
				Url = url
			};
			if (properties != null)
				foreach (KeyValuePair<string, string> pair in properties)
					requestTelemetry.Properties.Add(pair);
			string correlationId = SetCorrelationId(requestTelemetry.Properties);

			if (!string.IsNullOrWhiteSpace(userId))
				requestTelemetry.Context.User.Id = userId;
			if (!string.IsNullOrWhiteSpace(sessionId))
				requestTelemetry.Context.Session.Id = sessionId;

			TelemetryClient.Context.Operation.Name = name;
			requestTelemetry.Context.Operation.Id = correlationId;
			requestTelemetry.Context.Operation.Name = name;
			if (LoggerSettings != null)
			{
				requestTelemetry.Context.Cloud.RoleName = LoggerSettings.ModuleName;
				requestTelemetry.Context.Cloud.RoleInstance = LoggerSettings.Instance;
			}

			if (EnableThreadedOperations)
				ActionQueue.Enqueue(() =>
				{
					TelemetryClient.Context.Operation.Id = correlationId;
					TelemetryClient.Context.Operation.Name = name;
					ActionQueue.Enqueue(() => TelemetryClient.TrackRequest(requestTelemetry));
				});
			else
				TelemetryClient.TrackRequest(requestTelemetry);
		}

		/// <summary>
		/// Flushes the in-memory buffer, if one exists
		/// </summary>
		public virtual void Flush()
		{
			if (EnableThreadedOperations)
				ActionQueue.Enqueue(() => TelemetryClient.Flush());
			else
				TelemetryClient.Flush();
		}

		#endregion

		/// <summary>
		/// Sets the CorrelationId into the provided <paramref name="properties"/>
		/// </summary>
		/// <remarks>
		/// See https://dzimchuk.net/event-correlation-in-application-insights/ for details on correlating things together.
		/// </remarks>
		protected virtual string SetCorrelationId(IDictionary<string, string> properties)
		{
			try
			{
				string correlationId = CorrelationIdHelper.GetCorrelationId().ToString("N");
				properties["CorrelationId"] = correlationId;
				TelemetryClient.Context.Operation.Id = correlationId;
				return correlationId;
			}
			catch
			{
				return null;
			}
		}
	}
}