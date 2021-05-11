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
			var readers = await Task.WhenAll(await _from.GetTablesAsync(cancellationToken)
				.Select(x => MigrateAsync(x, cancellationToken)).ToListAsync(cancellationToken));
			foreach (var reader in readers)
				reader.Dispose();
		}

		private async Task<IDataReader> MigrateAsync(IDataReader reader, CancellationToken cc)
		{
			await _to.LoadTableAsync(reader, cc);
			return reader;
		}
	}
}
