using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Mtd.DbmsRandomizer.DatabaseManagement;
using Mtd.DbmsRandomizer.Query;
using MySql.Data.MySqlClient;

namespace Mtd.DbmsRandomizer.Mysql
{
	internal class MysqlQuerier : IQuerier
	{
		private readonly MySqlConnection _connection;

		public MysqlQuerier(MySqlConnection connection)
		{
			_connection = connection;
		}

		public async IAsyncEnumerable<T> ExecuteAsync<T>(string task, [EnumeratorCancellation] CancellationToken token) where T : new()
		{
			var command = new MySqlCommand(task, _connection);
			await using var reader = await command.ExecuteReaderAsync(token);
			while (await reader.ReadAsync(token))
			{
				// boxing messes it up for structs
				object result = new T();
				foreach (var field in typeof(T).GetProperties())
					field.SetValue(result, Convert.ChangeType(reader[field.Name], field.PropertyType));

				yield return (T)result;
			}
		}

		public Task ExecuteAsync(string task, CancellationToken token)
		{
			var command = new MySqlCommand(task, _connection);
			return command.ExecuteNonQueryAsync(token);
		}

		public DbType Type => DbType.MySql;
	}
}