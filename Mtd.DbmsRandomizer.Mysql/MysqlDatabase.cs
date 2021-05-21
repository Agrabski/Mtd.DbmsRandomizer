using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
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
			var file = GetTemporaryFilePath(reader);
			try
			{
				var bulkCopy = new MySqlBulkLoader(_connection)
				{
					TableName = reader.GetSchemaTable().TableName,
					FieldTerminator = ",",
					LineTerminator = "\n",
					FileName = file,

				};
			await bulkCopy.LoadAsync(cc);
			}
			finally
			{
				File.Delete(file);
			}
		}

		private string GetTemporaryFilePath(IDataReader reader)
		{
			var path = Path.GetRandomFileName();
			ToCsv(reader, path);
			return path;
		}

		public string FormatLiteral(object literal)
		{
			return new Random().Next(0, 1) == 0 ? $"'{literal}'": $"\"{literal}\"";
		}

		public DbConnection Connection => _connection;
		public DbType Type => DbType.MySql;

		public void ToCsv(IDataReader reader, string filename)
		{
			int nextResult = 0;
			do
			{
				var filePath = filename;
				using (var writer = new StreamWriter(filePath))
				{
					writer.WriteLine(string.Join(",", Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList()));
					int count = 0;
					while (reader.Read())
					{
						writer.WriteLine(string.Join(",", Enumerable.Range(0, reader.FieldCount).Select(reader.GetValue).ToList()));
						if (++count % 100 == 0)
						{
							writer.Flush();
						}
					}
				}

				filename = string.Format("{0}-{1}", filename, ++nextResult);
			}
			while (reader.NextResult());
		}
	}
}