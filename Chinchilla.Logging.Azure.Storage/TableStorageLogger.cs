﻿#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="Chinchilla Software Limited">
// // 	Copyright Chinchilla Software Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System;
using System.Configuration;
using System.Diagnostics;
using Azure.Core;
using Azure.Data.Tables;
using Chinchilla.Logging.Configuration;

#if NETSTANDARD2_0
#else
#endif

namespace Chinchilla.Logging.Azure.Storage
{
	/// <summary>
	/// Provides a set of methods that help you log events relating to the execution of your code outputting to an Azure Table Storage account
	/// SqlDatabaseTableName holds the name of the table
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
			string appSettingValue = GetSetting(container, containerLoggerSettings => containerLoggerSettings.LogsConnectionStringName, LoggerSettings.LogsConnectionStringName);
			if (string.IsNullOrWhiteSpace(appSettingValue))
				throw new ConfigurationErrorsException("No value for the setting 'LogsConnectionStringName' was provided");
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

				var tableClientOptions = new TableClientOptions();
				tableClientOptions.Retry.Mode = RetryMode.Exponential;
				tableClientOptions.Retry.Delay = TimeSpan.FromSeconds(10);
				tableClientOptions.Retry.MaxRetries = 6;
				var storageAccount = new TableServiceClient(connectionString, tableClientOptions);

				// Get a reference to the TableClient from the service client instance.
				TableClient tableClient = storageAccount.GetTableClient(tableName);

				// Create the table if it doesn't exist.
				tableClient.CreateIfNotExists();

				TEntity logEntity = ConvertLogInformation(logInformation);
				logEntity.Module = LoggerSettings.ModuleName;
				logEntity.Instance = LoggerSettings.Instance;
				logEntity.Environment = LoggerSettings.Environment;
				logEntity.EnvironmentInstance = LoggerSettings.EnvironmentInstance;

				tableClient.AddEntity(logEntity);
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
		public TableStorageLogger(ILoggerSettings loggerSettings, ICorrelationIdHelper correlationIdHelper, ITelemetryHelper telemetryHelper)
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