using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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

		public async IAsyncEnumerable<MigrationData> GetTablesAsync([EnumeratorCancellation] CancellationToken cc)
		{
			var schemaCommand = new MySqlCommand("select * from  information_schema.tables where TABLE_TYPE = 'BASE TABLE' and TABLE_SCHEMA not in ('mysql', 'sys', 'performance_schema')", _connection);
			var tableNames = new List<string>();
			await using (var schemaReader = await schemaCommand.ExecuteReaderAsync(cc))
			{

				while (await schemaReader.ReadAsync(cc))
				{
					tableNames.Add(schemaReader.GetString("TABLE_NAME"));
				}
			}
			foreach (var tableName in tableNames)
			{
				using var command = new MySqlCommand($"select * from {tableName}", _connection);

				var reader = await command.ExecuteReaderAsync(cc);
				yield return new MigrationData(tableName, reader);
			}
		}

		public async Task LoadTableAsync(IDataReader reader, string tableName, CancellationToken cc)
		{
			using var deleteCommand = _connection.CreateCommand();
			deleteCommand.CommandText = $"delete from {tableName}";
			await deleteCommand.ExecuteNonQueryAsync(cc);

			var t = await _connection.BeginTransactionAsync(cc);
			try
			{
				var commands = SplitIntoBatches(reader).Select(x => CreateCommand(x, GetColumnNames(reader), tableName, t));
				await Task.WhenAll(commands.Where(x => x != null).Select(x => x.ExecuteNonQueryAsync(cc)));
				foreach (var c in commands.Where(x => x != null))
					await c.DisposeAsync();
				await t.CommitAsync(cc);
			}
			catch (Exception)
			{
				await t.RollbackAsync();
				throw;
			}
		}


		public string FormatLiteral(object literal)
		{
			return new Random().Next(0, 1) == 0 ? $"'{literal}'" : $"\"{literal}\"";
		}

		public DbConnection Connection => _connection;
		public DbType Type => DbType.MySql;

		private IEnumerable<IEnumerable<object[]>> SplitIntoBatches(IDataReader reader)
		{
			var result = new List<object[]>();
			while (reader.Read())
			{
				if (result.Count > 100)
				{
					yield return result;
					result = new List<object[]>();
				}
				result.Add(Enumerable.Range(0, reader.FieldCount)
					.Select(reader.GetValue).ToArray());
			}
			yield return result;
		}

		private string[] GetColumnNames(IDataReader reader)
		{

			return Enumerable.Range(0, reader.FieldCount)
				.Select(reader.GetName).ToArray();

		}

		private MySqlCommand CreateCommand(IEnumerable<object[]> values, string[] names, string tableName, MySqlTransaction t)
		{
			if (!values.Any())
				return null;
			var v = string.Join(",\n", values.Select(x => $"({string.Join(", ", x.Select(FormatLiteral))})"));
			return new MySqlCommand(
				$"insert into " +
				$"{tableName} ({string.Join(", ", names)})" +
				$" values {v}", _connection, t);
		}
	}
}