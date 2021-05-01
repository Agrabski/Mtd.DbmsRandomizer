using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Mtd.DbmsRandomizer.DatabaseManagement;
using DbType = Mtd.DbmsRandomizer.DatabaseManagement.DbType;

namespace Mtd.DbmsRandomizer.Mssql
{
	internal class MSsqlDatabase : IDatabase
	{
		private readonly SqlConnection _connection;

		public MSsqlDatabase(SqlConnection connection)
		{
			_connection = connection;
		}

		public async IAsyncEnumerable<DataTable> GetTablesAsync(CancellationToken cc)
		{
			var schema = _connection.GetSchema();
			foreach (DataRow table in schema.Rows)
			{
				yield return await Task.Run(() =>
				{
					var result = new DataTable((string)table[2]);
					var adapter = new SqlDataAdapter($"select * from {table[2]}", _connection);
					adapter.Fill(result);
					return result;
				}, cc);
			}
		}

		public async Task LoadTableAsync(DataTable ds, CancellationToken cc)
		{
			var bulkCopy = new SqlBulkCopy(_connection);
			await bulkCopy.WriteToServerAsync(ds, cc);
		}

		public DbConnection Connection => _connection;
		public DbType Type => DbType.MsSql;
	}
}