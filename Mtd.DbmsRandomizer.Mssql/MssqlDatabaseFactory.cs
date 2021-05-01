using Mtd.DbmsRandomizer.DatabaseManagement;
using System.Data.SqlClient;
using DbType = Mtd.DbmsRandomizer.DatabaseManagement.DbType;

namespace Mtd.DbmsRandomizer.Mssql
{
	internal class MssqlDatabaseFactory : IDatabaseFactory
	{
		public DbType HandledType => DbType.MsSql;
		public IDatabase Create(string connectionString)
		{
			return new MSsqlDatabase(new SqlConnection(connectionString));
		}
	}
}
