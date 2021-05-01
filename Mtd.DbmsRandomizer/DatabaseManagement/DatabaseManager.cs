using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;
using Mtd.DbmsRandomizer.Query;
using Mtd.IOCUtility;
using Microsoft.Extensions.Options;

namespace Mtd.DbmsRandomizer.DatabaseManagement
{
	[Singleton]
	internal class DatabaseManager : IDatabaseManager, IQueryExecutor
	{
		private readonly DatabaseSwitchOptions _options;
		private readonly List<IDatabase> _connections = new();
		private readonly CancellationTokenSource _cancellationTokenSource = new();
		private readonly IDatabaseMigratorFactory _migratorFactory;
		private readonly IQuerierFactory _querierFactory;
		private readonly IDatabaseConnectionFactory _connectionFactory;
		private int _currentDatabaseIndex;
		private readonly AsyncReaderWriterLock _lock = new();

		public DatabaseManager(
			IOptions<DatabaseSwitchOptions> options,
			IDatabaseMigratorFactory migratorFactory,
			IQuerierFactory querierFactory, 
			IDatabaseConnectionFactory connectionFactory)
		{
			_options = options.Value;
			_migratorFactory = migratorFactory;
			_querierFactory = querierFactory;
			_connectionFactory = connectionFactory;
		}

		public void AppendConnection(DbConnection connection)
		{
			//_connections.Add(connection);
		}

		public void Start()
		{
			_connections.AddRange(_options.Databases.Select(_connectionFactory.Create));
			var token = _cancellationTokenSource.Token;
			_currentDatabaseIndex = 0;
			_ = Task.Run(async () =>
			  {
				  while (!token.IsCancellationRequested)
				  {
					  if (token.IsCancellationRequested)
						  return;
					  await Task.Delay(_options.SwitchInterval, token);
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

		public async IAsyncEnumerable<T> ExecuteAsync<T>(Func<IQuerier, IAsyncEnumerable<T>> task, CancellationToken token) where T : new()
		{
			await using (await _lock.ReadLockAsync(token))
				await foreach (var x in task(_querierFactory.Create(_connections[_currentDatabaseIndex])))
					yield return x;
		}
		public async Task ExecuteAsync(Func<IQuerier, Task> task, CancellationToken token)
		{
			await using (await _lock.ReadLockAsync(token))
				await task(_querierFactory.Create(_connections[_currentDatabaseIndex]));
		}

		public DbType DbmsType => _querierFactory.Create(_connections[_currentDatabaseIndex]).Type;
	}
}
