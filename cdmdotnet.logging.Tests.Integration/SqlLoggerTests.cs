using System.Collections.Generic;
using System.Threading;
using cdmdotnet.Logging;
using cdmdotnet.Logging.Azure.Configuration;
using cdmdotnet.Logging.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace cdmdotnet.logging.Tests.Integration
{
	/// <summary>
	/// A series of tests on the <see cref="SqlLogger"/> class
	/// </summary>
	[TestClass]
	public class SqlLoggerTests
	{
		[TestMethod]
		public void Dispose_100VariousLogsRaised_NoExceptionsThrown()
		{
			// Arrange
			var logger = new SqlLogger(new AzureLoggerSettingsConfiguration(), new NullCorrelationIdHelper());

			for (int i = 0; i < 100; i++)
			{
				switch (i % 5)
				{
					case 0:
						logger.LogDebug(string.Format("Test run index {0}", i), metaData: new Dictionary<string, object> {{"accountId", i}});
						break;
					case 1:
						logger.LogError(string.Format("Test run index {0}", i), metaData: new Dictionary<string, object> { { "teamId", i } });
						break;
					case 2:
						logger.LogFatalError(string.Format("Test run index {0}", i), metaData: new Dictionary<string, object> { { "userId", i } });
						break;
					case 3:
						logger.LogInfo(string.Format("Test run index {0}", i), metaData: new Dictionary<string, object> { { "companyId", i } });
						break;
					case 4:
						logger.LogError(string.Format("Test run index {0}", i), metaData: new Dictionary<string, object> { { "jobId", i } });
						break;
				}
			}

			// Act
			logger.Dispose();

			// Assert
			// No exception thrown
		}
		[TestMethod]
		public void PoolingThread_100LogsRaisedAndAPauseThenAnotherLog_PoolingThreadShutsDownAndStartsUpAgain()
		{
			// Arrange
			var logger = new SqlLogger(new TestApplicationSettings { EnableThreadedLogging = true, EnableDebug = true }, new NullCorrelationIdHelper());

			logger.LogDebug(string.Format("Test run index {0}", 0));
			logger.LogDebug(string.Format("Test run index {0}", 1));
			Thread.Sleep(5000);

			for (int i = 2; i < 100; i++)
				logger.LogDebug(string.Format("Test run index {0}", i));
			Thread.Sleep(5000);

			// Act
			logger.LogDebug(string.Format("Test run index {0}", 100));

			// Assert
			// No exception thrown and disposing
			logger.Dispose();
		}
	}

	public class TestApplicationSettings : ILoggerSettings
	{
		public TestApplicationSettings()
		{
			SqlDatabaseLogsConnectionStringName = "SqlDatabaseLogs";
			SqlDatabaseTableName = "Logs";
			ModuleName = "Tests";
			Instance = "Test Instance";
			Environment = "Test Environment";
			EnvironmentInstance = "Test Environment Instance";
		}

		#region Implementation of ILoggerSettings

		/// <summary>
		/// If false <see cref="M:cdmdotnet.Logging.ILogger.LogSensitive(System.String,System.String,System.Exception,System.Collections.Generic.IDictionary{System.String,System.Object},System.Collections.Generic.IDictionary{System.String,System.Object})"/> will not do anything nor log anything.
		/// </summary>
		public bool EnableSensitive { get; set; }

		/// <summary>
		/// If false <see cref="M:cdmdotnet.Logging.ILogger.LogInfo(System.String,System.String,System.Exception,System.Collections.Generic.IDictionary{System.String,System.Object},System.Collections.Generic.IDictionary{System.String,System.Object})"/> will not do anything nor log anything.
		/// </summary>
		public bool EnableInfo { get; set; }

		/// <summary>
		/// If false <see cref="M:cdmdotnet.Logging.ILogger.LogProgress(System.String,System.String,System.Exception,System.Collections.Generic.IDictionary{System.String,System.Object},System.Collections.Generic.IDictionary{System.String,System.Object})"/> will not do anything nor log anything.
		/// </summary>
		public bool EnableProgress { get; set; }

		/// <summary>
		/// If false <see cref="M:cdmdotnet.Logging.ILogger.LogDebug(System.String,System.String,System.Exception,System.Collections.Generic.IDictionary{System.String,System.Object},System.Collections.Generic.IDictionary{System.String,System.Object})"/> will not do anything nor log anything.
		/// </summary>
		public bool EnableDebug { get; set; }

		/// <summary>
		/// If false <see cref="M:cdmdotnet.Logging.ILogger.LogWarning(System.String,System.String,System.Exception,System.Collections.Generic.IDictionary{System.String,System.Object},System.Collections.Generic.IDictionary{System.String,System.Object})"/> will not do anything nor log anything.
		/// </summary>
		public bool EnableWarning { get; set; }

		/// <summary>
		/// If false <see cref="M:cdmdotnet.Logging.ILogger.LogError(System.String,System.String,System.Exception,System.Collections.Generic.IDictionary{System.String,System.Object},System.Collections.Generic.IDictionary{System.String,System.Object})"/> will not do anything nor log anything.
		/// </summary>
		public bool EnableError { get; set; }

		/// <summary>
		/// If false <see cref="M:cdmdotnet.Logging.ILogger.LogFatalError(System.String,System.String,System.Exception,System.Collections.Generic.IDictionary{System.String,System.Object},System.Collections.Generic.IDictionary{System.String,System.Object})"/> will not do anything nor log anything.
		/// </summary>
		public bool EnableFatalError { get; set; }

		/// <summary>
		/// If true, the <see cref="T:cdmdotnet.Logging.ILogger"/> will use one extra thread per instance to persist logs. 
		///             This means the log methods like <see cref="M:cdmdotnet.Logging.ILogger.LogInfo(System.String,System.String,System.Exception,System.Collections.Generic.IDictionary{System.String,System.Object},System.Collections.Generic.IDictionary{System.String,System.Object})"/> will return indicating the information is in a queue to be logged.
		///             This greatly increases the performance in the event your <see cref="T:cdmdotnet.Logging.ILogger"/> is under heavy load, for example, if your logging database is under strain, your application will continue to perform, but logs will be queued.
		/// </summary>
		public bool EnableThreadedLogging { get; set; }

		/// <summary>
		/// Doesn't do anything just yet
		/// </summary>
		public bool EnableThreadedLoggingOutput { get; set; }

		/// <summary>
		/// A friendly identifier to help identify different applications if they use the same <see cref="T:cdmdotnet.Logging.ILogger"/>.
		/// </summary>
		public string ModuleName { get; set; }

		/// <summary>
		/// A friendly identifier to help identify different instances of the same application, such as a development or production instance of the same application.
		/// </summary>
		public string Instance { get; set; }

		/// <summary>
		/// A friendly identifier to help identify different environments of the same application, such as deployments to different geo-graphical locations of the same application.
		/// </summary>
		public string EnvironmentInstance { get; set; }

		/// <summary/>
		public string Environment { get; set; }

		/// <summary>
		/// The key of the <see cref="P:System.Configuration.ConfigurationManager.AppSettings"/> item that holds the name of the connection string to use.
		/// </summary>
		public string SqlDatabaseLogsConnectionStringName { get; set; }

		/// <summary>
		/// The name of the table to use.
		/// </summary>
		public string SqlDatabaseTableName { get; set; }

		#endregion
	}
}
