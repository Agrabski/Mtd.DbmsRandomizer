using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Threading;
using Mtd.DbmsRandomizer.Query;
using Mtd.IOCUtility;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Mtd.DbmsRandomizer.DatabaseManagement
{
	[Singleton]
	[UsedImplicitly]
	internal sealed class DatabaseManager : IDatabaseManager, IQueryExecutor
	{
		private readonly DatabaseSwitchOptions _options;
		private readonly List<IDatabase> _connections = new();
		private readonly CancellationTokenSource _cancellationTokenSource = new();
		private readonly IDatabaseMigratorFactory _migratorFactory;
		private readonly List<IQuerierFactory> _querierFactory;
		private readonly IDatabaseConnectionFactory _connectionFactory;
		private int _currentDatabaseIndex;
		private int _queriesToSwitch = 0;
		private readonly AsyncReaderWriterLock _lock = new();

		public DatabaseManager(
			IOptions<DatabaseSwitchOptions> options,
			IDatabaseMigratorFactory migratorFactory,
			IEnumerable<IQuerierFactory> querierFactory,
			IDatabaseConnectionFactory connectionFactory)
		{
			_options = options.Value;
			_migratorFactory = migratorFactory;
			_querierFactory = querierFactory.ToList();
			_connectionFactory = connectionFactory;
		}

		public void AppendConnection(DbConnection connection)
		{
			//_connections.Add(connection);
		}

		public async Task StartAsync()
		{
			if (_options.Databases.Count < 2)
				throw new InvalidOperationException(
					$"Too few database connections for switching. Was {_options.Databases.Count} shoud be at least 2");
			_connections.AddRange(_options.Databases.Select(_connectionFactory.Create));
			await Task.WhenAll(_connections.Select(x => x.Connection.OpenAsync()));
			var token = _cancellationTokenSource.Token;
			_currentDatabaseIndex = 0;
			_queriesToSwitch = SelectNewInterval();
		}

		public IDatabaseAccessContext Context => new DatabaseAccessContext(this);

		public void Dispose()
		{
			_cancellationTokenSource.Cancel();
			_cancellationTokenSource.Dispose();
		}

		private async Task SwitchDatabaseAsync(CancellationToken token)
		{
			if (_queriesToSwitch-- <= 0)
			{
				await using (await _lock.WriteLockAsync(token))
				{
					var from = _connections[_currentDatabaseIndex];
					int newDatabaseIndex;
					var random = new Random();
					do
					{
						newDatabaseIndex = random.Next(0, _connections.Count);
					} while (newDatabaseIndex == _currentDatabaseIndex);

					_currentDatabaseIndex = newDatabaseIndex;
					var to = _connections[_currentDatabaseIndex];
					var migrator = _migratorFactory.Create(from, to);
					await migrator.MigrateAsync(token);
					_queriesToSwitch = SelectNewInterval();
				}
			}
		}

		public async IAsyncEnumerable<T> ExecuteAsync<T>(Func<IQuerier, IAsyncEnumerable<T>> task, [EnumeratorCancellation] CancellationToken token) where T : new()
		{
			await using (await _lock.ReadLockAsync(token))
				await foreach (var x in task(_querierFactory.First(x=>x.HandledType == _connections[_currentDatabaseIndex].Type).Create(_connections[_currentDatabaseIndex])))
					yield return x;
			await SwitchDatabaseAsync(new ());
		}
		public async Task ExecuteAsync(Func<IQuerier, Task> task, CancellationToken token)
		{
			await using (await _lock.ReadLockAsync(token))
				await task(_querierFactory.First(x => x.HandledType == _connections[_currentDatabaseIndex].Type).Create(_connections[_currentDatabaseIndex]));
			await SwitchDatabaseAsync(new());
		}

		public DbType DbmsType => _querierFactory.First(x => x.HandledType == _connections[_currentDatabaseIndex].Type).Create(_connections[_currentDatabaseIndex]).Type;

		private int SelectNewInterval()
		{
			return new Random().Next(_options.MinimumSwitchInterval, _options.MaximumSwitchInterval);
		}

		public string FromatLiteral(object literal)
		{
			return _connections[_currentDatabaseIndex].FormatLiteral(literal);
		}
	}
}
