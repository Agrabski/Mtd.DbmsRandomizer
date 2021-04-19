using System.Threading;
using System.Threading.Tasks;
using Mtd.DbmsRandomizer.DatabaseManagement;

namespace Mtd.DbmsRandomizer.Query
{
	internal interface IQuerier
	{
		Task<T> ExecuteAsync<T>(string task, CancellationToken token);
		Task ExecuteAsync(string task, CancellationToken token);

		DbType Type { get; }
	}
}