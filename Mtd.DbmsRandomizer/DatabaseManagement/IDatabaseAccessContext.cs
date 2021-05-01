using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Mtd.DbmsRandomizer.DatabaseManagement
{
	public interface IDatabaseAccessContext
	{
		ILiteral CreateLiteral(string value);
		ILiteral CreateLiteral(DateTime value);

		IAsyncEnumerable<T> ExecuteQueryAsync<T>(string query, CancellationToken token, params ILiteral[] literals) where T : new();

		Task ExecuteNonQueryAsync(string nonQuery, CancellationToken token, params ILiteral[] literals);
	}
}