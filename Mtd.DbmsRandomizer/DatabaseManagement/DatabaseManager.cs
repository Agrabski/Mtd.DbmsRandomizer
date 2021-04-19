using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;
using Mtd.DbmsRandomizer.Query;
using Mtd.IOCUtility;

namespace Mtd.DbmsRandomizer.DatabaseManagement
{
	[Singleton]
	internal class DatabaseManager : IDatabaseManager, IQueryExecutor
	{
		private readonly TimeSpan _databaseSwitchSpan;
		private readonly List<DbConnection> _connections = new();
		private readonly CancellationTokenSource _cancellationTokenSource = new();
		private readonly IDatabaseMigratorFactory _migratorFactory;
		private readonly IQuerierFactory _querierFactory;
		private int _currentDatabaseIndex;
		private readonly AsyncReaderWriterLock _lock = new();

		public DatabaseManager(TimeSpan databaseSwitchSpan, IDatabaseMigratorFactory migratorFactory, IQuerierFactory querierFactory)
		{
			_databaseSwitchSpan = databaseSwitchSpan;
			_migratorFactory = migratorFactory;
			_querierFactory = querierFactory;
		}

		public void AppendConnection(DbConnection connection)
		{
			_connections.Add(connection);
		}

		public void Start()
		{
			var token = _cancellationTokenSource.Token;
			_currentDatabaseIndex = 0;
			_ = Task.Run(async () =>
			  {
				  while (!token.IsCancellationRequested)
				  {
					  if (token.IsCancellationRequested)
						  return;
					  await Task.Delay(_databaseSwitchSpan, token);
					  await SwitchDatabaseAsync(token);
				  }
			  }, token);
		}

		public IDatabaseAccessContext Context => new DatabaseAccessContext(this);

		public void Dispose()
		{
			_cancellationTokenSource.Cancel();
			_cancellationTokenSource.Dispose();
			GC.SuppressFinalize(this);
		}

		private async Task SwitchDatabaseAsync(CancellationToken token)
		{
			await using (await _lock.WriteLockAsync(token))
			{
				var from = _connections[_currentDatabaseIndex];
				_currentDatabaseIndex = _currentDatabaseIndex > _connections.Count ? 0 : _currentDatabaseIndex + 1;
				var to = _connections[_currentDatabaseIndex];
				var migrator = _migratorFactory.Create(from, to);
				await migrator.MigrateAsync(token);
			}
		}

		public async Task<T> Execute<T>(Func<IQuerier, Task<T>> task, CancellationToken token)
		{
			await using (await _lock.ReadLockAsync(token))
				return await task(_querierFactory.Create(_connections[_currentDatabaseIndex]));
		}
		public async Task Execute(Func<IQuerier, Task> task, CancellationToken token)
		{
			await using (await _lock.ReadLockAsync(token))
				await task(_querierFactory.Create(_connections[_currentDatabaseIndex]));
		}

		public DbType DbmsType => _querierFactory.Create(_connections[_currentDatabaseIndex]).Type;
	}
}
