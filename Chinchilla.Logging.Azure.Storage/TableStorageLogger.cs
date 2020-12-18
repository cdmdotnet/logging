using System;
using System.Configuration;
using System.Diagnostics;
using Chinchilla.Logging.Configuration;
using Microsoft.Azure.Cosmos.Table;

#if NETSTANDARD2_0
#else
#endif

namespace Chinchilla.Logging.Azure.Storage
{
	/// <summary>
	/// Provides a set of methods that help you log events relating to the execution of your code outputting to an Azure Table Storage account
	/// </summary>
	public abstract class TableStorageLogger<TEntity>
		: Logger
		where TEntity : LogEntity
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="TableStorageLogger{TEntity}"/> class calling the constructor on <see cref="Logger"/>.
		/// </summary>
		protected TableStorageLogger(ILoggerSettings loggerSettings, ICorrelationIdHelper correlationIdHelper, ITelemetryHelper telemetryHelper)
			: base(loggerSettings, correlationIdHelper, telemetryHelper)
		{
		}

		/// <summary />
		protected virtual string GetConnectionString(string container)
		{
			string appSettingValue = GetSetting(container, containerLoggerSettings => containerLoggerSettings.SqlDatabaseLogsConnectionStringName, LoggerSettings.SqlDatabaseLogsConnectionStringName);
			if (string.IsNullOrWhiteSpace(appSettingValue))
				throw new ConfigurationErrorsException("No value for the setting 'SqlDatabaseLogsConnectionStringName' was provided");
			string connectionString = LoggerSettings.GetConnectionString(appSettingValue);
			return connectionString;
		}

		/// <summary>
		/// Converts the provided <paramref name="logInformation"/> into a <typeparamref name="TEntity"/>
		/// </summary>
		protected abstract TEntity ConvertLogInformation(LogInformation logInformation);

		/// <summary>
		/// Persists (or saves) the provided <paramref name="logInformation"></paramref> to the database
		/// </summary>
		/// <param name="logInformation">The <see cref="LogInformation"/> holding all the information you want to persist (save) to the database.</param>
		protected override void PersistLog(LogInformation logInformation)
		{
			try
			{
				string tableName = GetSetting(logInformation.Container, containerLoggerSettings => containerLoggerSettings.SqlDatabaseTableName, LoggerSettings.SqlDatabaseTableName);
				string connectionString = GetConnectionString(logInformation.Container);

				CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
				CloudTableClient tblclient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
				CloudTable table = tblclient.GetTableReference(tableName);
				table.CreateIfNotExists();

				TEntity logEntity = ConvertLogInformation(logInformation);
				logEntity.Module = LoggerSettings.ModuleName;
				logEntity.Instance = LoggerSettings.Instance;
				logEntity.Environment = LoggerSettings.Environment;
				logEntity.EnvironmentInstance = LoggerSettings.EnvironmentInstance;

				TableOperation insertOperation = TableOperation.Insert(logEntity);
				table.Execute(insertOperation);
			}
			catch (Exception exception)
			{
				Trace.TraceError("Persisting log failed with the following exception:\r\n{0}\r\n{1}", exception.Message, exception.StackTrace);
			}
		}
	}

	/// <summary>
	/// Provides a set of methods that help you log events relating to the execution of your code outputting to an Azure Table Storage account
	/// </summary>
	public class TableStorageLogger
		: TableStorageLogger<LogEntity>
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="TableStorageLogger{TEntity}"/> class calling the constructor on <see cref="Logger"/>.
		/// </summary>
		protected TableStorageLogger(ILoggerSettings loggerSettings, ICorrelationIdHelper correlationIdHelper, ITelemetryHelper telemetryHelper)
			: base(loggerSettings, correlationIdHelper, telemetryHelper)
		{
		}

		/// <summary>
		/// Converts the provided <paramref name="logInformation"/> into a <see cref="LogEntity"/>
		/// </summary>
		protected override LogEntity ConvertLogInformation(LogInformation logInformation)
		{
			return new LogEntity(logInformation);
		}
	}
}