using System.Data.SqlClient;
using Mtd.DbmsRandomizer.DatabaseManagement;
using Mtd.DbmsRandomizer.Query;
using MySql.Data.MySqlClient;

namespace Mtd.DbmsRandomizer.Mysql
{
	public class MysqlQuerierFactory : IQuerierFactory
	{
		public DbType HandledType => DbType.MySql;

		public IQuerier Create(IDatabase connection)
		{
			return new MysqlQuerier(connection.Connection as MySqlConnection);
		}
	}
}