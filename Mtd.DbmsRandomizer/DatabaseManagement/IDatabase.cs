using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Mtd.DbmsRandomizer.DatabaseManagement
{
	public interface IDatabase
	{
		IAsyncEnumerable<IDataReader> GetTablesAsync(CancellationToken cc);
		Task LoadTableAsync(IDataReader reader, CancellationToken cc);
		DbConnection Connection { get; }
		DbType Type { get; }

		string FormatLiteral(object literal);
	}
}
