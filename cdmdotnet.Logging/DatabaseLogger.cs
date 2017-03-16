#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="cdmdotnet Limited">
// // 	Copyright cdmdotnet Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using cdmdotnet.Logging.Configuration;
using Newtonsoft.Json;

namespace cdmdotnet.Logging
{
	/// <summary>
	/// Provides a set of methods that help you log events relating to the execution of your code outputting to a database
	/// </summary>
	public abstract class DatabaseLogger : Logger
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="DatabaseLogger"/> class calling the constructor on <see cref="Logger"/>.
		/// </summary>
		protected DatabaseLogger(ILoggerSettings loggerSettings, ICorrelationIdHelper correlationIdHelper)
			: base(loggerSettings, correlationIdHelper)
		{
		}

		/// <summary />
		protected virtual string GetSqlConnectionString()
		{
			string appSettingValue = LoggerSettings.SqlDatabaseLogsConnectionStringName;
			if (string.IsNullOrWhiteSpace(appSettingValue))
				throw new ConfigurationErrorsException("No value for the setting 'SqlDatabaseLogsConnectionStringName' was provided");
			ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[LoggerSettings.SqlDatabaseLogsConnectionStringName];
			if (connectionStringSettings == null)
				throw new ConfigurationErrorsException(string.Format("No connection string named '{0}' was provided", appSettingValue));
			string connectionString = connectionStringSettings.ConnectionString;
			if (string.IsNullOrWhiteSpace(appSettingValue))
				throw new ConfigurationErrorsException(string.Format("No value for the connection string named '{0}' was provided", appSettingValue));
			return connectionString;
		}

		/// <summary />
		protected abstract IDbConnection GetDbConnection(string connectionString);

		/// <summary />
		protected abstract IDbCommand GetCommand();

		/// <summary />
		protected abstract IDbTransaction GetWriteTransaction(IDbConnection dbConnection);

		/// <summary>
		/// Persists (or saves) the provided <paramref name="logInformation"></paramref> to the database
		/// </summary>
		/// <param name="logInformation">The <see cref="LogInformation"/> holding all the information you want to persist (save) to the database.</param>
		protected override void PersistLog(LogInformation logInformation)
		{
			try
			{
				using (IDbConnection dbConnection = GetDbConnection(GetSqlConnectionString()))
				{
					dbConnection.Open();
					IDbCommand command = GetCommand();
					command.Connection = dbConnection;
					command.CommandType = CommandType.Text;

					using (IDbTransaction dbTransaction = GetWriteTransaction(dbConnection))
					{
						command.Transaction = dbTransaction;

						command.CommandText = GetInsertStatement();

						IDbDataParameter raisedParameter = command.CreateParameter();
						raisedParameter.DbType = DbType.DateTime;
						raisedParameter.Direction = ParameterDirection.Input;
						raisedParameter.ParameterName = "@Raised";
						raisedParameter.Value = logInformation.Raised;
						command.Parameters.Add(raisedParameter);

						IDbDataParameter moduleParameter = command.CreateParameter();
						moduleParameter.DbType = DbType.String;
						moduleParameter.Direction = ParameterDirection.Input;
						moduleParameter.ParameterName = "@Module";
						moduleParameter.Value = LoggerSettings.ModuleName;
						command.Parameters.Add(moduleParameter);

						IDbDataParameter instanceParameter = command.CreateParameter();
						instanceParameter.DbType = DbType.String;
						instanceParameter.Direction = ParameterDirection.Input;
						instanceParameter.ParameterName = "@Instance";
						instanceParameter.Value = LoggerSettings.Instance;
						command.Parameters.Add(instanceParameter);

						IDbDataParameter environmentParameter = command.CreateParameter();
						environmentParameter.DbType = DbType.String;
						environmentParameter.Direction = ParameterDirection.Input;
						environmentParameter.ParameterName = "@Environment";
						environmentParameter.Value = LoggerSettings.Environment;
						command.Parameters.Add(environmentParameter);

						IDbDataParameter environmentInstanceParameter = command.CreateParameter();
						environmentInstanceParameter.DbType = DbType.String;
						environmentInstanceParameter.Direction = ParameterDirection.Input;
						environmentInstanceParameter.ParameterName = "@EnvironmentInstance";
						environmentInstanceParameter.Value = LoggerSettings.EnvironmentInstance;
						command.Parameters.Add(environmentInstanceParameter);

						IDbDataParameter correlationIdParameter = command.CreateParameter();
						correlationIdParameter.DbType = DbType.Guid;
						correlationIdParameter.Direction = ParameterDirection.Input;
						correlationIdParameter.ParameterName = "@CorrelationId";
						correlationIdParameter.Value = logInformation.CorrolationId;
						command.Parameters.Add(correlationIdParameter);

						IDbDataParameter levelParameter = command.CreateParameter();
						levelParameter.DbType = DbType.String;
						levelParameter.Direction = ParameterDirection.Input;
						levelParameter.ParameterName = "@Level";
						levelParameter.Value = logInformation.Level ?? (object)DBNull.Value;
						command.Parameters.Add(levelParameter);

						IDbDataParameter messageParameter = command.CreateParameter();
						messageParameter.DbType = DbType.String;
						messageParameter.Direction = ParameterDirection.Input;
						messageParameter.ParameterName = "@Message";
						messageParameter.Value = logInformation.Message ?? (object)DBNull.Value;
						command.Parameters.Add(messageParameter);

						IDbDataParameter containerParameter = command.CreateParameter();
						containerParameter.DbType = DbType.String;
						containerParameter.Direction = ParameterDirection.Input;
						containerParameter.ParameterName = "@Container";
						containerParameter.Value = logInformation.Container ?? (object)DBNull.Value;
						command.Parameters.Add(containerParameter);

						IDbDataParameter exceptionParameter = command.CreateParameter();
						exceptionParameter.DbType = DbType.String;
						exceptionParameter.Direction = ParameterDirection.Input;
						exceptionParameter.ParameterName = "@Exception";
						exceptionParameter.Value = logInformation.Exception ?? (object)DBNull.Value;
						command.Parameters.Add(exceptionParameter);

						if (logInformation.AdditionalData != null)
						{
							foreach (string key in logInformation.AdditionalData.Keys)
							{
								object value = logInformation.AdditionalData[key];
								IDbDataParameter additionalDataParameter = command.CreateParameter();
								additionalDataParameter.DbType = DbType.String;
								additionalDataParameter.Direction = ParameterDirection.Input;
								additionalDataParameter.ParameterName = string.Format("@{0}", key);
								additionalDataParameter.Value = value != null ? JsonConvert.SerializeObject(value) : (object)DBNull.Value;
								command.Parameters.Add(additionalDataParameter);

								string tableName = LoggerSettings.SqlDatabaseTableName;
								command.CommandText = command.CommandText
									.Replace(", " + tableName + ".MetaData", string.Format(", " + tableName +".MetaData, " + tableName + ".{0}", key))
									.Replace(", @MetaData", string.Format(", @MetaData, @{0}", key));
							}
						}

						IDbDataParameter metaDataParameter = command.CreateParameter();
						metaDataParameter.DbType = DbType.String;
						metaDataParameter.Direction = ParameterDirection.Input;
						metaDataParameter.ParameterName = "@MetaData";
						metaDataParameter.Value = logInformation.MetaData ?? (object)DBNull.Value;
						command.Parameters.Add(metaDataParameter);

						command.ExecuteNonQuery();

						dbTransaction.Commit();
					}
				}
			}
			catch (Exception exception)
			{
				Trace.TraceError("Persisting log failed with the following exception:\r\n{0}\r\n{1}", exception.Message, exception.StackTrace);
			}
		}

		/// <summary>
		/// Creates the required table for storing logs in
		/// </summary>
		protected virtual void CreateTable()
		{
			try
			{
				using (IDbConnection dbConnection = GetDbConnection(GetSqlConnectionString()))
				{
					dbConnection.Open();
					IDbCommand command = GetCommand();
					command.Connection = dbConnection;
					command.CommandType = CommandType.Text;

					using (IDbTransaction dbTransaction = GetWriteTransaction(dbConnection))
					{
						command.Transaction = dbTransaction;

						command.CommandText = GetCreateTableStatement();

						command.ExecuteNonQuery();

						dbTransaction.Commit();
					}
				}
			}
			catch (Exception exception)
			{
				Trace.TraceError("Creating logging table failed with the following exception:\r\n{0}\r\n{1}", exception.Message, exception.StackTrace);
			}
		}

		/// <summary>
		/// Something similar to
		/// INSERT INTO Logs
		/// (Logs.Raised, Logs.Level, Logs.Module, Logs.Instance, Logs.Environment, Logs.EnvironmentInstance, Logs.CorrelationId, Logs.Message, Logs.Container, Logs.Exception)
		/// VALUES
		/// (@Raised, @Level, @Module, @Instance, @Environment, @EnvironmentInstance, @CorrelationId,@Message, @Container, @Exception);
		/// </summary>
		protected virtual string GetInsertStatement()
		{
			string tableName = LoggerSettings.SqlDatabaseTableName;
			return string.Format(@"INSERT INTO {0}
({0}.Raised, {0}.Level, {0}.Module, {0}.Instance, {0}.Environment, {0}.EnvironmentInstance, {0}.CorrelationId, {0}.Message, {0}.Container, {0}.Exception, {0}.MetaData)
VALUES
(@Raised, @Level, @Module, @Instance, @Environment, @EnvironmentInstance, @CorrelationId, @Message, @Container, @Exception, @MetaData);", tableName);
		}

		/// <summary />
		protected virtual string GetCreateTableStatement()
		{
			string tableName = LoggerSettings.SqlDatabaseTableName;
			return string.Format(@"IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE name='{0}' and xtype='U')
BEGIN
	CREATE TABLE {0}
	(
		Id int NOT NULL IDENTITY (1, 1),
		Module nvarchar(255) NOT NULL,
		Instance nvarchar(255) NOT NULL,
		Environment nvarchar(255) NOT NULL,
		EnvironmentInstance nvarchar(255) NOT NULL,
		Raised datetime NOT NULL,
		[Level] nvarchar(50) NOT NULL,
		CorrelationId uniqueidentifier NOT NULL,
		Message nvarchar(MAX) NULL,
		Container nvarchar(MAX) NULL,
		Exception nvarchar(MAX) NULL,
		MetaData nvarchar(MAX) NULL
	);

	ALTER TABLE {0} ADD CONSTRAINT
	PK_{0} PRIMARY KEY CLUSTERED 
	(
		Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);

	CREATE NONCLUSTERED INDEX IX_{0}_Level ON {0}
	(
		[Level]
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);

	CREATE NONCLUSTERED INDEX IX_{0}_CorrelationId ON dbo.{0}
	(
		CorrelationId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);

	CREATE NONCLUSTERED INDEX IX_{0}_Raised ON dbo.{0}
	(
		Raised
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);

	CREATE NONCLUSTERED INDEX IX_{0}_Module ON {0}
	(
		[Module]
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);

	CREATE NONCLUSTERED INDEX IX_{0}_Instance ON {0}
	(
		[Instance]
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);

	CREATE NONCLUSTERED INDEX IX_{0}_Environment ON {0}
	(
		[Environment]
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);

	CREATE NONCLUSTERED INDEX IX_{0}_EnvironmentInstance ON {0}
	(
		[EnvironmentInstance]
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);

	ALTER TABLE dbo.{0} SET (LOCK_ESCALATION = TABLE);

	PRINT('Logs table {0} created.')
END", tableName);
		}
	}
}