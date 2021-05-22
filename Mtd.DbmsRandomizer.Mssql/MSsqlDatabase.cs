using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
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

		public async IAsyncEnumerable<MigrationData> GetTablesAsync([EnumeratorCancellation] CancellationToken cc)
		{
			var schemaCommand = new SqlCommand("select * from sys.objects where type_desc = 'USER_TABLE'", _connection);
			await using var schemaReader = await schemaCommand.ExecuteReaderAsync(cc);

			while(await schemaReader.ReadAsync(cc))
			{
				var tableName = schemaReader.GetString("name");
				var command = new SqlCommand($"select * from {tableName}", _connection);

				var reader = await command.ExecuteReaderAsync(cc);
				yield return new MigrationData(tableName, reader);
			}
		}

		public async Task LoadTableAsync(IDataReader reader, string tableName, CancellationToken cc)
		{
			var deleteCommand = _connection.CreateCommand();
			deleteCommand.CommandText = $"delete from {tableName}";
			await deleteCommand.ExecuteNonQueryAsync(cc);
			var bulkCopy = new SqlBulkCopy(_connection)
			{
				DestinationTableName = tableName
			};
			await bulkCopy.WriteToServerAsync(reader, cc);
		}

		public string FormatLiteral(object literal)
		{
			return new Random().Next(0, 1) == 0 ? $"'{literal}'": $"\"{literal}\"";
		}

		public DbConnection Connection => _connection;
		public DbType Type => DbType.MsSql;
	}
}