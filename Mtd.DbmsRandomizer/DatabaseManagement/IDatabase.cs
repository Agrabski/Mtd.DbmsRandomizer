using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Mtd.DbmsRandomizer.DatabaseManagement
{
	public interface IDatabase
	{
		IAsyncEnumerable<DataTable> GetTablesAsync(CancellationToken cc);
		Task LoadTableAsync(DataTable ds, CancellationToken cc);
		DbConnection Connection { get; }
		DbType Type { get; }
	}
}
