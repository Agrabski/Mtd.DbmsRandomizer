using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mtd.IOCUtility;

namespace Mtd.DbmsRandomizer.DatabaseManagement
{
	[DontRegister]
	internal class DatabaseMigrator : IDatabaseMigrator
	{
		private readonly IDatabase _from;
		private readonly IDatabase _to;

		public DatabaseMigrator(IDatabase @from, IDatabase to)
		{
			_from = @from;
			_to = to;
		}

		public async Task MigrateAsync(CancellationToken cancellationToken)
		{
			await Task.WhenAll(await _from.GetTablesAsync(cancellationToken)
				.Select(x => MigrateAsync(x.Reader, x.TableName, cancellationToken)).ToListAsync(cancellationToken));
		}

		private async Task MigrateAsync(IDataReader reader, string tableName, CancellationToken cc)
		{
			await _to.LoadTableAsync(reader, tableName, cc);
			reader.Dispose();
		}
	}
}
