using System.Data.Common;
using Mtd.DbmsRandomizer.DatabaseManagement;
using System.Data.SqlClient;

namespace Mtd.DbmsRandomizer.Mssql
{
	internal class MssqlDatabaseFactory : IDatabaseFactory
	{
		public DbType HandledType => DbType.MsSql;
		public DbConnection Create(string connectionString)
		{
			return new SqlConnection(connectionString);
		}
	}
}
