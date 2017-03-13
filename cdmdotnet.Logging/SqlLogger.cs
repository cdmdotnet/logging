#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="cdmdotnet Limited">
// // 	Copyright cdmdotnet Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System.Data;
using System.Data.SqlClient;
using cdmdotnet.Logging.Configuration;

namespace cdmdotnet.Logging
{
	/// <summary>
	/// Provides a set of methods that help you log events relating to the execution of your code outputting to a SqlServer database.
	/// </summary>
	/// <remarks>
	/// If the table created is not suitable, override the methods that generate the sql statements.
	/// </remarks>
	public class SqlLogger : DatabaseLogger
	{
		#region Overrides of DatabaseLock

		/// <summary>
		/// Instantiates a new instance of the <see cref="DatabaseLogger"/> class calling the constructor on <see cref="DatabaseLogger"/>.
		/// </summary>
		public SqlLogger(ILoggerSettings loggerSettings, ICorrelationIdHelper correlationIdHelper)
			: base(loggerSettings, correlationIdHelper)
		{
		}

		/// <summary>
		/// Instantiates a new instance of the <see cref="DatabaseLogger"/> class calling the constructor on <see cref="DatabaseLogger"/>.
		/// This will attempt to create the table in the database if not yet created and <paramref name="createTable"/> is true.
		/// </summary>
		public SqlLogger(ILoggerSettings loggerSettings, ICorrelationIdHelper correlationIdHelper, bool createTable)
			: this(loggerSettings, correlationIdHelper)
		{
			if (createTable)
				CreateTable();
		}

		/// <summary />
		protected override IDbConnection GetDbConnection(string connectionString)
		{
			return new SqlConnection(connectionString);
		}

		/// <summary />
		protected override IDbCommand GetCommand()
		{
			return new SqlCommand();
		}

		/// <summary />
		protected override IDbTransaction GetWriteTransaction(IDbConnection dbConnection)
		{
			return ((SqlConnection)dbConnection).BeginTransaction(IsolationLevel.ReadUncommitted);
		}

		#endregion
	}
}