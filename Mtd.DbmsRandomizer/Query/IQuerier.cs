using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mtd.DbmsRandomizer.DatabaseManagement;

namespace Mtd.DbmsRandomizer.Query
{
	public interface IQuerier
	{
		IAsyncEnumerable<T> ExecuteAsync<T>(string task, CancellationToken token) where T : new();
		Task ExecuteAsync(string task, CancellationToken token);

		DbType Type { get; }
	}
}