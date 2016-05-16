﻿#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="cdmdotnet Limited">
// // 	Copyright cdmdotnet Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System;
using System.Data;
using System.Diagnostics;
using cdmdotnet.Logging.Configuration;

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
		protected abstract string GetSqlConnectionString();

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
						Guid corrolationId = Guid.Empty;
						try
						{
							corrolationId = CorrelationIdHelper.GetCorrelationId();
						}
						catch { }
						correlationIdParameter.Value = corrolationId;
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
			return @"INSERT INTO Logs
(Logs.Raised, Logs.Level, Logs.Module, Logs.Instance, Logs.Environment, Logs.EnvironmentInstance, Logs.CorrelationId, Logs.Message, Logs.Container, Logs.Exception)
VALUES
(@Raised, @Level, @Module, @Instance, @Environment, @EnvironmentInstance, @CorrelationId, @Message, @Container, @Exception);";
		}

		/// <summary />
		protected virtual string GetCreateTableStatement()
		{
			return @"IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE name='Logs' and xtype='U')
BEGIN
	CREATE TABLE Logs
	(
		Id int NOT NULL IDENTITY (1, 1),
		Module nvarchar(50) NOT NULL,
		Instance nvarchar(50) NOT NULL,
		Environment nvarchar(50) NOT NULL,
		EnvironmentInstance nvarchar(50) NOT NULL,
		Raised datetime NOT NULL,
		[Level] nvarchar(50) NOT NULL,
		CorrelationId uniqueidentifier NOT NULL,
		Message nvarchar(MAX) NULL,
		Container nvarchar(MAX) NULL,
		Exception nvarchar(MAX) NULL
	);

	ALTER TABLE Logs ADD CONSTRAINT
	PK_Logs PRIMARY KEY CLUSTERED 
	(
		Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);

	CREATE NONCLUSTERED INDEX IX_Logs_Level ON Logs
	(
		[Level]
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);

	CREATE NONCLUSTERED INDEX IX_Logs_CorrelationId ON dbo.Logs
	(
		CorrelationId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);

	CREATE NONCLUSTERED INDEX IX_Logs_Raised ON dbo.Logs
	(
		Raised
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);

	CREATE NONCLUSTERED INDEX IX_Logs_Module ON Logs
	(
		[Module]
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);

	CREATE NONCLUSTERED INDEX IX_Logs_Instance ON Logs
	(
		[Instance]
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);

	CREATE NONCLUSTERED INDEX IX_Logs_Environment ON Logs
	(
		[Environment]
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);

	CREATE NONCLUSTERED INDEX IX_Logs_EnvironmentInstance ON Logs
	(
		[EnvironmentInstance]
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);

	ALTER TABLE dbo.Logs SET (LOCK_ESCALATION = TABLE);

	PRINT('Logs table created.')
END";
		}
	}
}