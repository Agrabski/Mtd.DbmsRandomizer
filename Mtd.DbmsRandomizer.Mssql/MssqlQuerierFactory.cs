﻿using System.Data.SqlClient;
using Mtd.DbmsRandomizer.DatabaseManagement;
using Mtd.DbmsRandomizer.Query;

namespace Mtd.DbmsRandomizer.Mssql
{
	public class MssqlQuerierFactory : IQuerierFactory
	{
		public IQuerier Create(IDatabase connection)
		{
			return new MssqlQuerier(connection.Connection as SqlConnection);
		}
	}
}