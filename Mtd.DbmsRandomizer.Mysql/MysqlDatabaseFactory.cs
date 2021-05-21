using Mtd.DbmsRandomizer.DatabaseManagement;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using DbType = Mtd.DbmsRandomizer.DatabaseManagement.DbType;

namespace Mtd.DbmsRandomizer.Mysql
{
	internal class MysqlDatabaseFactory : IDatabaseFactory
	{
		public DbType HandledType => DbType.MySql;
		public IDatabase Create(string connectionString)
		{
			return new MysqlDatabase(new MySqlConnection(connectionString));
		}
	}
}
