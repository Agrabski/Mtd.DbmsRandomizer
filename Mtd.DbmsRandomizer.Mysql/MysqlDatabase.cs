using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Mtd.DbmsRandomizer.DatabaseManagement;
using MySql.Data.MySqlClient;
using DbType = Mtd.DbmsRandomizer.DatabaseManagement.DbType;

namespace Mtd.DbmsRandomizer.Mysql
{
	internal class MysqlDatabase : IDatabase
	{
		private readonly MySqlConnection _connection;

		public MysqlDatabase(MySqlConnection connection)
		{
			_connection = connection;
		}

		public async IAsyncEnumerable<IDataReader> GetTablesAsync([EnumeratorCancellation] CancellationToken cc)
		{
			var schema = _connection.GetSchema();
			foreach (DataRow table in schema.Rows)
			{
				var command = new MySqlCommand($"select * from {table[2]}", _connection);
				yield return await command.ExecuteReaderAsync(cc);
			}
		}

		public async Task LoadTableAsync(IDataReader reader, CancellationToken cc)
		{
			var deleteCommand = _connection.CreateCommand();
			deleteCommand.CommandText = $"delete from {reader.GetSchemaTable().TableName}";
			await deleteCommand.ExecuteNonQueryAsync(cc);
			var bulkCopy = new MySqlBulkLoader(_connection);
			await bulkCopy.LoadAsync(reader, cc);
		}

		public string FormatLiteral(object literal)
		{
			return new Random().Next(0, 1) == 0 ? $"'{literal}'": $"\"{literal}\"";
		}

		public DbConnection Connection => _connection;
		public DbType Type => DbType.MySql;
	}
}