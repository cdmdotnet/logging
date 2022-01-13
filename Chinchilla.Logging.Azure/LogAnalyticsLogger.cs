#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="Chinchilla Software Limited">
// // 	Copyright Chinchilla Software Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

#if NET40
#else

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Chinchilla.Logging.Azure.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Chinchilla.Logging.Azure
{
	/// <summary>
	/// Provides a set of methods that help you log events relating to the execution of your code outputting to an Azure Log Analytics.
	/// </summary>
	public class LogAnalyticsLogger
		: Logger
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="LogAnalyticsLogger"/> class calling the constructor on <see cref="Logger"/>.
		/// </summary>
		public LogAnalyticsLogger(ILogAnalyticsSettings loggerSettings, ICorrelationIdHelper correlationIdHelper, ITelemetryHelper telemetryHelper)
			: base(loggerSettings, correlationIdHelper, telemetryHelper)
		{
			LogAnalyticsSettings = loggerSettings;

			DefaultJsonSerializerSettings = new JsonSerializerSettings
			{
				Formatting = Formatting.None,
				MissingMemberHandling = MissingMemberHandling.Ignore,
				DateParseHandling = DateParseHandling.DateTimeOffset,
				DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
				Converters = new List<JsonConverter> { new StringEnumConverter() },
				DateFormatHandling = DateFormatHandling.IsoDateFormat,
				DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
				FloatFormatHandling = FloatFormatHandling.DefaultValue,
				NullValueHandling = NullValueHandling.Ignore,
				PreserveReferencesHandling = PreserveReferencesHandling.None,
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
				StringEscapeHandling = StringEscapeHandling.EscapeNonAscii,
				TypeNameHandling = TypeNameHandling.All,
			};
		}

		/// <summary>
		/// The <see cref="ILogAnalyticsSettings"/> for the instance, set during Instantiation
		/// </summary>
		protected ILogAnalyticsSettings LogAnalyticsSettings { get; set; }

		/// <summary>
		/// Build the signature to include as part of the request.
		/// see https://docs.microsoft.com/en-us/azure/azure-monitor/logs/data-collector-api#authorization.
		/// </summary>
		protected virtual string BuildSignature(LogAnalyticsInformation logInformation, string body)
		{
			byte[] bodyBytes = Encoding.UTF8.GetBytes(body);

			string xHeaders = $"x-ms-date:{logInformation.Raised:R}";
			string stringToHash = $"POST\n{bodyBytes.Length}\napplication/json\n{xHeaders}\n/api/logs";
			byte[] messageBytes = Encoding.ASCII.GetBytes(stringToHash);

			byte[] keyBytes = Convert.FromBase64String(LogAnalyticsSettings.GetSharedKey(logInformation.Container));

			using (var hmacsha256 = new HMACSHA256(keyBytes))
			{
				byte[] calculatedHash = hmacsha256.ComputeHash(messageBytes);
				string encodedHash = Convert.ToBase64String(calculatedHash);
				string authorization = $"{LogAnalyticsSettings.GetWorkspaceId(logInformation.Container)}:{encodedHash}";

				return authorization;
			}
		}

		/// <summary>
		/// Persists (or saves) the provided <paramref name="logInformation"></paramref> to the database
		/// </summary>
		/// <param name="logInformation">The <see cref="LogInformation"/> holding all the information you want to persist (save) to the database.</param>
		protected override void PersistLog(LogInformation logInformation)
		{
			try
			{
				var logAnalyticsInformation = new LogAnalyticsInformation(LogAnalyticsSettings, logInformation);
				string payloadBody = JsonConvert.SerializeObject(logAnalyticsInformation, DefaultJsonSerializerSettings);
				string signature = BuildSignature(logAnalyticsInformation, payloadBody);

				var executeTask = Task.Factory.StartNew(async () => {
					try
					{
						string url = "api/logs?api-version=2016-04-01";
						var content = new StringContent(payloadBody, Encoding.UTF8);
						content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

						using (var client = new HttpClient())
						{
							client.BaseAddress = new Uri($"https://{LogAnalyticsSettings.GetWorkspaceId(logAnalyticsInformation.Container)}.ods.opinsights.azure.com/");
							client.DefaultRequestHeaders.Accept.Clear();
							client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
							client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("SharedKey", signature);

							client.DefaultRequestHeaders.Add("Log-Type", LogAnalyticsSettings.GetLogType(logAnalyticsInformation.Container));
							client.DefaultRequestHeaders.Add("x-ms-date", logAnalyticsInformation.Raised.ToString("R"));
							client.DefaultRequestHeaders.Add("time-generated-field", "");

							HttpResponseMessage response = client.PostAsync(url, content).Result;
							if (response.StatusCode != HttpStatusCode.OK)
							{
								HttpContent responseContent = response.Content;
								string result = responseContent.ReadAsStringAsync().Result;
								Trace.TraceError("Persisting log failed with the following exception:\r\n{0}", result);
							}
						}
					}
					catch (Exception exception)
					{
						Trace.TraceError("Persisting log failed with the following exception:\r\n{0}\r\n{1}", exception.Message, exception.StackTrace);
					}
				});

				if (!GetSetting(logAnalyticsInformation.Container, containerLoggerSettings => containerLoggerSettings.EnableThreadedLogging, LoggerSettings.EnableThreadedLogging))
					executeTask.Wait();
			}
			catch (Exception exception)
			{
				Trace.TraceError("Persisting log failed with the following exception:\r\n{0}\r\n{1}", exception.Message, exception.StackTrace);
			}
		}
	}
}
#endif