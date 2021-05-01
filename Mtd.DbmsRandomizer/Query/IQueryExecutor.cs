using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mtd.DbmsRandomizer.DatabaseManagement;

namespace Mtd.DbmsRandomizer.Query
{
	internal interface IQueryExecutor
	{
		IAsyncEnumerable<T> ExecuteAsync<T>(Func<IQuerier, IAsyncEnumerable<T>> task, CancellationToken token) where T : new();
		Task ExecuteAsync(Func<IQuerier, Task> task, CancellationToken token);
		DbType DbmsType { get; }
	}
}