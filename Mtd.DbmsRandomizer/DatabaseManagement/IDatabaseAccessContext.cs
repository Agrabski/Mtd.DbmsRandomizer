using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Mtd.DbmsRandomizer.DatabaseManagement
{
	public interface IDatabaseAccessContext
	{
		IAsyncEnumerable<T> ExecuteQueryAsync<T>(string query, CancellationToken token, params object[] literals) where T : new();

		Task ExecuteNonQueryAsync(string nonQuery, CancellationToken token, params object[] literals);
	}
}