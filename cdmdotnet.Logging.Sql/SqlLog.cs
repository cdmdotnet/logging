#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="cdmdotnet Limited">
// // 	Copyright cdmdotnet Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using cdmdotnet.Logging.Configuration;

namespace cdmdotnet.Logging.Sql
{
	/// <remarks>
	/// If the table created is not suitable, override the methods that generate the sql statements.
	/// </remarks>
	public class SqlLogger : DatabaseLogger
	{
		#region Overrides of DatabaseLock

		public SqlLogger(ILoggerSettings loggerSettings, ICorrelationIdHelper correlationIdHelper)
			: base(loggerSettings, correlationIdHelper)
		{
		}

		/// <summary>
		/// Will attempt to create the table in the database if not yet created.
		/// </summary>
		public SqlLogger(ILoggerSettings loggerSettings, ICorrelationIdHelper correlationIdHelper, bool createTable)
			: this(loggerSettings, correlationIdHelper)
		{
			if (createTable)
				CreateTable();
		}

		protected override string GetSqlConnectionString()
		{
			return ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["SqlDatabaseLogsConnectionStringName"]].ConnectionString;
		}

		protected override IDbConnection GetDbConnection(string connectionString)
		{
			return new SqlConnection(connectionString);
		}

		protected override IDbCommand GetCommand()
		{
			return new SqlCommand();
		}

		protected override IDbTransaction GetWriteTransaction(IDbConnection dbConnection)
		{
			return ((SqlConnection)dbConnection).BeginTransaction(IsolationLevel.ReadUncommitted);
		}

		#endregion

		#region Overrides of Logger

		protected override string GetQueueThreadName()
		{
			return "Sql Database Log queue polling";
		}

		#endregion
	}
}