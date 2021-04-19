using System;
using System.Threading;
using System.Threading.Tasks;
using Mtd.DbmsRandomizer.DatabaseManagement;

namespace Mtd.DbmsRandomizer.Query
{
	internal interface IQueryExecutor
	{
		Task<T> Execute<T>(Func<IQuerier, Task<T>> task, CancellationToken token);
		Task Execute(Func<IQuerier,Task> task, CancellationToken token);
		DbType DbmsType { get;}
	}
}