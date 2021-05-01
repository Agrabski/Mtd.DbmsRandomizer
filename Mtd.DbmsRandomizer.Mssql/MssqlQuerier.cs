﻿using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Mtd.DbmsRandomizer.DatabaseManagement;
using Mtd.DbmsRandomizer.Query;

namespace Mtd.DbmsRandomizer.Mssql
{
	internal class MssqlQuerier : IQuerier
	{
		private readonly SqlConnection _connection;

		public MssqlQuerier(SqlConnection connection)
		{
			_connection = connection;
		}

		public async IAsyncEnumerable<T> ExecuteAsync<T>(string task, [EnumeratorCancellation] CancellationToken token) where T : new()
		{
			var command = new SqlCommand(task, _connection);
			var reader = await command.ExecuteReaderAsync(token);
			while (await reader.ReadAsync(token))
			{
				var result = new T();
				foreach (var field in typeof(T).GetFields())
				{
					field.SetValue(result, reader[field.Name]);
				}

				yield return result;
			}
		}

		public Task ExecuteAsync(string task, CancellationToken token)
		{
			var command = new SqlCommand(task, _connection);
			return command.ExecuteNonQueryAsync(token);
		}

		public DbType Type => DbType.MsSql;
	}
}