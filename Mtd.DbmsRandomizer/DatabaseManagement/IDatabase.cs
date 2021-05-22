using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Mtd.DbmsRandomizer.DatabaseManagement
{
	public class MigrationData
	{
		public IDataReader Reader { get; }
		public string TableName { get; }

		public MigrationData(string tableName, IDataReader reader)
		{
			TableName = tableName;
			Reader = reader;
		}
	}

	public interface IDatabase
	{
		IAsyncEnumerable<MigrationData> GetTablesAsync(CancellationToken cc);
		Task LoadTableAsync(IDataReader reader, string tableName, CancellationToken cc);
		DbConnection Connection { get; }
		DbType Type { get; }

		string FormatLiteral(object literal);
	}
}
